using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Src.Utils.UI
{
    /// TextMeshPro elements don't seem to set their PreferredSize correctly
    ///     causing surrounding LayoutGroups to shrink them too much
    ///     define a LayoutElement with a higher priority that just sets the PreferredSize
    ///     to the MinSize.
    [ExecuteInEditMode]
    [RequireComponent(typeof(TextMeshProUGUI))]
    [RequireComponent(typeof(RectTransform))]
    public class TextMeshProCustomLayoutElement : MonoBehaviour, ILayoutElement
    {
        public TextMeshProUGUI TextMeshProTarget;
        public bool PreferredWidthIsMinWidth;
        public bool PreferredHeightIsMinHeight;

        public float preferredWidth => TextMeshProTarget.preferredWidth;

        public float preferredHeight => TextMeshProTarget.preferredHeight;

        public float minWidth => PreferredWidthIsMinWidth ? TextMeshProTarget.preferredWidth : TextMeshProTarget.minWidth;

        public float minHeight => PreferredHeightIsMinHeight ? TextMeshProTarget.preferredHeight : TextMeshProTarget.minHeight;

        public float flexibleWidth => TextMeshProTarget.flexibleWidth;

        public float flexibleHeight => TextMeshProTarget.flexibleHeight;

        public int layoutPriority => TextMeshProTarget.layoutPriority + 1;

        private void OnEnable()
        {
            TextMeshProTarget = GetComponent<TextMeshProUGUI>();
        }

        public void CalculateLayoutInputHorizontal()
        {
            TextMeshProTarget.CalculateLayoutInputHorizontal();
        }

        public void CalculateLayoutInputVertical()
        {
            TextMeshProTarget.CalculateLayoutInputVertical();
        }

        private void OnValidate()
        {
            LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }
    }
}