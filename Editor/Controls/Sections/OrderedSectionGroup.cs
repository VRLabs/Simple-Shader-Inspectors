using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace VRLabs.SimpleShaderInspectors.Controls.Sections
{
    /// <summary>
    /// Control that contains a list of OrderedSections and controls thir lifecycle
    /// </summary>
    public class OrderedSectionGroup : SimpleControl, IControlContainer
    {
        private List<SimpleControl> _controls;

        private bool? areNewSectionsAvailable;

        /// <summary>
        /// List of available Ordered Sections
        /// </summary>
        public List<OrderedSection> Sections { get; set; }

        /// <summary>
        /// GUIStyle for the add button
        /// </summary>
        public GUIStyle ButtonStyle { get; set; }

        /// <summary>
        /// Color of the add button.
        /// </summary>
        public Color ButtonColor { get; set; }
        /// <summary>
        /// LIst of controls. You can't use it normally.
        /// </summary>
        public List<SimpleControl> Controls
        {
            get
            {
                List<SimpleControl> l = new List<SimpleControl>();
                l.AddRange(_controls);
                l.AddRange(Sections);
                return l;
            }
            set => _controls = value;
        }

        /// <summary>
        ///  Default contructor of <see cref="OrderedSectionGroup"/>.
        /// </summary>
        /// <param name="alias">Alias of the control</param>
        public OrderedSectionGroup(string alias) : base(alias)
        {
            _controls = new List<SimpleControl>();
            Sections = new List<OrderedSection>();

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
            for (int i = 0; i < Sections.Count; i++)
            {
                Sections[i].PredrawUpdate(materialEditor);
            }
            if (areNewSectionsAvailable == null)
            {
                UpdateSectionsOrder();
            }
            for (int i = 0; i < Sections.Count; i++)
            {
                if (Sections[i].Enabled)
                {
                    Sections[i].DrawControl(materialEditor);
                    if (Sections[i].PushState != 0)
                    {
                        if (Sections[i].PushState == 1 && i < Sections.Count - 1)
                        {
                            Sections[i].AdditionalProperties[0].Property.floatValue++;
                            Sections[i + 1].AdditionalProperties[0].Property.floatValue--;
                        }
                        else if (Sections[i].PushState == -1 && i > 0 && Sections[i - 1].Enabled)
                        {
                            Sections[i].AdditionalProperties[0].Property.floatValue--;
                            Sections[i - 1].AdditionalProperties[0].Property.floatValue++;
                        }
                        Sections[i].PushState = 0;
                        needsOrderUpdate = true;
                    }
                    else if (Sections[i].HasActivatePropertyUpdated)
                    {
                        needsSectionAvailabilityUpdate = true;
                    }
                }
            }
            if (areNewSectionsAvailable == null || needsSectionAvailabilityUpdate)
            {
                areNewSectionsAvailable = AreNewSectionsAvailable();
            }
            if (needsOrderUpdate)
            {
                UpdateSectionsOrder();
            }
            EditorGUILayout.Space();
            DrawAddButton();
        }

        private void DrawAddButton()
        {
            if (areNewSectionsAvailable ?? true)
            {
                Color bCol = GUI.backgroundColor;
                GUI.backgroundColor = ButtonColor;
                bool buttonPressed = GUILayout.Button(Content, ButtonStyle);
                if (buttonPressed)
                {
                    GenericMenu menu = new GenericMenu();
                    foreach (OrderedSection section in Sections)
                    {
                        if (HasAtLeastOneDisabled(section))
                        {
                            menu.AddItem(section.Content, false, TurnOnSection, section);
                        }
                    }
                    menu.ShowAsContext();
                }
                GUI.backgroundColor = bCol;
            }
        }

        /// <summary>
        /// Turns on a section, setting it's index to the best number
        /// </summary>
        /// <param name="sectionVariable">The section to turn on</param>
        private void TurnOnSection(object sectionVariable)
        {
            OrderedSection section = (OrderedSection)sectionVariable;
            section.AdditionalProperties[0].Property.floatValue = 753;
            section.HasSectionTurnedOn = true;
            areNewSectionsAvailable = AreNewSectionsAvailable();
            UpdateSectionsOrder();
        }

        private void UpdateSectionsOrder()
        {
            Sections.Sort(CompareSectionsOrder);
            int j = 1;
            foreach (OrderedSection section in Sections)
            {
                if (section.AdditionalProperties[0].Property.floatValue != 0 && !section.AdditionalProperties[0].Property.hasMixedValue)
                {
                    section.AdditionalProperties[0].Property.floatValue = j;
                    j++;
                }
            }
        }

        private bool AreNewSectionsAvailable()
        {
            bool yesThereAre = false;
            foreach (OrderedSection section in Sections)
            {
                yesThereAre = HasAtLeastOneDisabled(section);
                if (yesThereAre) break;
            }
            return yesThereAre;
        }

        private static bool HasAtLeastOneDisabled(OrderedSection section)
        {
            bool yesItHas = false;
            foreach (Material mat in section.AdditionalProperties[0].Property.targets)
            {
                yesItHas = mat.GetFloat(section.AdditionalProperties[0].Property.name) == 0;
                if (yesItHas) break;
            }
            return yesItHas;
        }

        /// <summary>
        /// Compares 2 ordered sections to determine which one is the first one
        /// </summary>
        /// <param name="x">First section to compare</param>
        /// <param name="y">Second section to compare</param>
        /// <returns></returns>
        private static int CompareSectionsOrder(OrderedSection x, OrderedSection y)
        {
            if (x == null)
            {
                if (y == null) return 0;
                else return -1;
            }
            else
            {
                if (y == null)
                {
                    return 1;
                }
                else
                {
                    if (x.AdditionalProperties[0].Property.floatValue > y.AdditionalProperties[0].Property.floatValue)
                    {
                        return 1;
                    }
                    else if (x.AdditionalProperties[0].Property.floatValue < y.AdditionalProperties[0].Property.floatValue)
                    {
                        return -1;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
        }
    }
}