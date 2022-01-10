using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls.Sections
{
    /// <summary>
    /// Control that contains a list of OrderedSections and manages their lifecycle.
    /// </summary>
    /// <remarks>
    /// <para>The main purpose of this control is to manage multiple <see cref="OrderedSection"/> controls, reordering them when needed, and provide a button to enable disabled ones</para>
    /// <para>In practice this control is what makes the entire ordered section system work, and that's the reason to why controls of type <see cref="OrderedSection"/> can only live inside this control.</para>
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// OrderedSectionGroup group = this.AddOrderedSectionGroup("GroupAlias");
    ///  
    /// group.AddOrderedSection("_ActivateProperty1");
    /// group.AddOrderedSection("_ActivateProperty2");
    /// group.AddOrderedSection("_ActivateProperty3");
    /// </code>
    /// </example>
    public class OrderedSectionGroup : SimpleControl, IControlContainer<OrderedSection>
    {
        private bool? _areNewSectionsAvailable;

        /// <summary>
        /// List of available Ordered Sections.
        /// </summary>
        /// <value>
        /// A list of <see cref="OrderedSection"/> containing the sections this control manages.
        /// </value>
        public List<OrderedSection> Controls { get; set; }

        /// <summary>
        /// Style for the add button.
        /// </summary>
        /// <value>
        /// GUIStyle used when displaying the add button.
        /// </value>
        public GUIStyle ButtonStyle { get; set; }

        /// <summary>
        /// Color of the add button.
        /// </summary>
        /// <value>
        /// Color used when displaying the add button.
        /// </value>
        public Color ButtonColor { get; set; }

        /// <summary>
        ///  Default constructor of <see cref="OrderedSectionGroup"/>.
        /// </summary>
        /// <param name="alias">Alias of the control</param>
        public OrderedSectionGroup(string alias) : base(alias)
        {
            Controls = new List<OrderedSection>();

            ButtonStyle = new GUIStyle(Styles.Bubble);
            ButtonColor = Color.white;
        }

        /// <summary>
        /// Draws the group of sections.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            bool needsOrderUpdate = false;
            bool needsSectionAvailabilityUpdate = false;
            foreach (var t in Controls)
                t.PredrawUpdate(materialEditor);

            if (_areNewSectionsAvailable == null)
                UpdateSectionsOrder();
            
            for (int i = 0; i < Controls.Count; i++)
            {
                if (!Controls[i].Enabled) continue;
                
                Controls[i].DrawControl(materialEditor);
                if (Controls[i].PushState != 0)
                {
                    if (Controls[i].PushState == 1 && i < Controls.Count - 1)
                    {
                        Controls[i].SectionPosition++;
                        Controls[i + 1].SectionPosition--;
                    }
                    else if (Controls[i].PushState == -1 && i > 0 && Controls[i - 1].Enabled)
                    {
                        Controls[i].SectionPosition--;
                        Controls[i - 1].SectionPosition++;
                    }
                    Controls[i].PushState = 0;
                    needsOrderUpdate = true;
                }
                else if (Controls[i].HasActivatePropertyUpdated)
                {
                    needsSectionAvailabilityUpdate = true;
                }
            }
            if (_areNewSectionsAvailable == null || needsSectionAvailabilityUpdate)
                _areNewSectionsAvailable = AreNewSectionsAvailable();
            
            if (needsOrderUpdate)
                UpdateSectionsOrder();
            
            EditorGUILayout.Space();
            DrawAddButton();
        }

        //draw the add button and handles the dropdown when pressed
        private void DrawAddButton()
        {
            if (!(_areNewSectionsAvailable ?? true)) return;
            
            Color bCol = GUI.backgroundColor;
            GUI.backgroundColor = ButtonColor;
            var buttonRect = GUILayoutUtility.GetRect(Content, ButtonStyle);
            bool buttonPressed = GUI.Button(buttonRect, Content, ButtonStyle);
            
            if (buttonPressed)
            {
                List<OrderedSection> items = new List<OrderedSection>();
                foreach (var section in Controls)
                    if (section.HasAtLeastOneMaterialDisabled())
                        items.Add(section);
                
                var dropdown = new OrderedSectionDropdown(Content.text, new AdvancedDropdownState(), items, TurnOnSection);
                dropdown.Show(buttonRect);
            }
            GUI.backgroundColor = bCol;
        }

        /// <summary>
        /// Turns on a section, setting its index to the best number
        /// </summary>
        /// <param name="section">The section to turn on</param>
        private void TurnOnSection(OrderedSection section)
        {
            section.SectionPosition = 753;
            section.HasSectionTurnedOn = true;
            _areNewSectionsAvailable = AreNewSectionsAvailable();
            UpdateSectionsOrder();
        }

        private void UpdateSectionsOrder()
        {
            Controls.Sort(CompareSectionsOrder);
            int j = 1;
            foreach (var section in Controls)
            {
                if (section.SectionPosition != 0 && !section.AdditionalProperties[0].Property.hasMixedValue)
                {
                    section.SectionPosition = j;
                    j++;
                }
            }
        }

        private bool AreNewSectionsAvailable()
        {
            bool yesThereAre = false;
            foreach (var section in Controls)
            {
                yesThereAre = section.HasAtLeastOneMaterialDisabled();
                if (yesThereAre) break;
            }
            return yesThereAre;
        }

        /// <summary>
        /// Compares 2 ordered sections to determine which one is the first one
        /// </summary>
        /// <param name="x">First section to compare</param>
        /// <param name="y">Second section to compare</param>
        /// <returns></returns>
        private static int CompareSectionsOrder(OrderedSection x, OrderedSection y)
        {
            if (x == null) return y == null ? 0 : -1;

            if (y == null) return 1;
            if (x.SectionPosition > y.SectionPosition)
                return 1;
            if (x.SectionPosition < y.SectionPosition)
                return -1;
            return 0;
        }
        
        /// <summary>
        /// Implementation needed by <see cref="IControlContainer{T}"/> to get the object's controls list.
        /// </summary>
        /// <returns><see cref="Controls"/></returns>
        public IEnumerable<OrderedSection> GetControlList() => Controls;

        /// <summary>
        /// Implementation needed by <see cref="IControlContainer{T}"/> to add controls. All controls added are stored in <see cref="Controls"/>
        /// </summary>
        /// <param name="control">Control to add.</param>
        /// <param name="alias">Optional alias to say where a control is appended after.</param>
        public void AddControl(OrderedSection control, string alias = "") => Controls.AddControl(control, alias);

        /// <summary>
        /// Implementation needed by <see cref="IControlContainer"/> to add controls. All controls added are stored in <see cref="Controls"/> only if the parameter
        /// is of type <see cref="OrderedSection"/>
        /// </summary>
        /// <param name="control">Control to add.</param>
        /// <param name="alias">Optional alias to say where a control is appended after.</param>
        void IControlContainer.AddControl(SimpleControl control, string alias = "")
        {
            if(control is OrderedSection section)
                Controls.AddControl(section, alias);
        }

        /// <summary>
        /// Implementation needed by <see cref="IControlContainer"/> to get the object's controls list.
        /// </summary>
        /// <returns><see cref="Controls"/></returns>
        IEnumerable<SimpleControl> IControlContainer.GetControlList() => Controls;
    }
}