using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls.Sections
{
    /// <summary>
    /// Dropdown used by the <see cref="OrderedSectionGroup"/> and used to give the user a searchable dropdown to enable disabled sections.
    /// </summary>
    public class OrderedSectionDropdown : AdvancedDropdown
    {
        private readonly Action<OrderedSection> _turnOnSection;
        private readonly List<OrderedSection> _sections;
        private readonly string _name;

        /// <summary>
        /// Main constructor of the <see cref="OrderedSectionDropdown"/>
        /// </summary>
        /// <param name="name">name visualized in the header</param>
        /// <param name="state">Advanced dropdown state</param>
        /// <param name="sections">enumerable of <see cref="OrderedSection"/> to display in the dropdown</param>
        /// <param name="turnOnSection">Function to call when selecting a section</param>
        public OrderedSectionDropdown(string name, AdvancedDropdownState state, IEnumerable<OrderedSection> sections, Action<OrderedSection> turnOnSection) : base(state)
        {
            minimumSize = new Vector2(270, 300);
            _turnOnSection = turnOnSection;
            _sections = sections.ToList();
            _name = name;
        }
    
        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem(_name);

            _sections.Sort((s1, s2) =>
            {
                int order; 
                
                var s1Parts = s1.CustomPopupPath?.Split('/') ?? Array.Empty<string>();
                var s2Parts = s2.CustomPopupPath?.Split('/') ?? Array.Empty<string>();
                for (int i = 0; i < Math.Min(s2Parts.Length, s2Parts.Length); i++)
                {
                    bool s1Empty = string.IsNullOrWhiteSpace(s1Parts[i]);
                    bool s2Empty = string.IsNullOrWhiteSpace(s2Parts[i]);
                    if (s1Empty && !s2Empty) return 1;
                    if (s2Empty && !s1Empty) return -1;
                    order = string.Compare(s1Parts[i], s2Parts[i], StringComparison.Ordinal);
                    if(order != 0) return order;
                }

                order = s2Parts.Length.CompareTo(s1Parts.Length);

                if (order != 0) return order;

                order = string.Compare(s1.Content.text, s2.Content.text, StringComparison.Ordinal);
                return order;
            });

            foreach (OrderedSection section in _sections)
                AddSectionToMenu(root, section);
            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            if (item is SectionDropdownItem secDrop)
                _turnOnSection(secDrop.Section);
        }
        
        private void AddSectionToMenu(AdvancedDropdownItem root, OrderedSection section)
        {
            var parent = root;
            var shaderNameParts = section.CustomPopupPath?.Split('/');
            if (shaderNameParts != null)
                foreach (string folderName in shaderNameParts)
                    parent = FindOrCreateChild(parent, folderName);

            parent.AddChild(new SectionDropdownItem(section));
            
        }
        
        private AdvancedDropdownItem FindOrCreateChild(AdvancedDropdownItem parent, string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return parent;
            foreach (var child in parent.children)
            {
                if (child.name == name)
                    return child;
            }

            var item = new AdvancedDropdownItem(name);
            parent.AddChild(item);
            return item;
        }
    }

    /// <summary>
    /// Dropdown item used by <see cref="OrderedSectionDropdown"/> for its items
    /// </summary>
    public class SectionDropdownItem : AdvancedDropdownItem
    {
        public readonly OrderedSection Section;
        /// <summary>
        /// Main constructor of <see cref="SectionDropdownItem"/>
        /// </summary>
        /// <param name="section"><see cref="OrderedSection"/> this item represents</param>
        public SectionDropdownItem(OrderedSection section) : base(section.Content.text)
        {
            Section = section;
        }
    }
}