using System;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class PointsUIManager : MonoBehaviour
{
        private UIDocument _document;
        private Label _p1Label;
        private Label _p2Label;
        private Label _p3Label;
        
        private VisualElement _h1;
        private VisualElement _h2;
        private VisualElement _h3;

        private VisualElement _p2H1;
        private VisualElement _p2H2;
        private VisualElement _p2H3;

        private VisualElement _p3H1;
        private VisualElement _p3H2;
        private VisualElement _p3H3;

        private void Awake()
        {
                _document = GetComponent<UIDocument>();
                GameManager_v2.Instance.OnUISelected.AddListener(OnUISelected);
                GameManager_v2.Instance.OnPointsUpdated.AddListener(OnPointsUpdated);
                GameManager_v2.Instance.OnGameLost.AddListener(() => { _document.rootVisualElement.visible = false; });
        }

        private void OnUISelected()
        {
                VisualElement root = _document.rootVisualElement;
                _p1Label = root.Q<Label>("P1_PointLabel");
                _p2Label = root.Q<Label>("P2_PointLabel");
                _p3Label = root.Q<Label>("P3_PointLabel");
                
                _h1 = root.Q<VisualElement>("H1");
                _h2 = root.Q<VisualElement>("H2");
                _h3 = root.Q<VisualElement>("H3");
                
                _p2H1 = root.Q<VisualElement>("p2H1");
                _p2H2 = root.Q<VisualElement>("p2H2");
                _p2H3 = root.Q<VisualElement>("p2H3");
                
                _p3H1 = root.Q<VisualElement>("p3H1");
                _p3H2 = root.Q<VisualElement>("p3H2");
                _p3H3 = root.Q<VisualElement>("p3H3");
                
                GameManager_v2.Instance.HeartsChanged.AddListener(SetHearts);
        }

        private void SetVisibleIfAvailable(VisualElement elem, bool visible)
        {
                if(elem != null) elem.visible = visible;
        }

        public void SetHearts(int number)
        {
                switch (number)
                {
                        case 0:
                                SetVisibleIfAvailable(_h1, false);
                                SetVisibleIfAvailable(_p2H1, false);
                                SetVisibleIfAvailable(_p3H1, false);
                                break;
                        case 1:
                                SetVisibleIfAvailable(_h2, false);
                                SetVisibleIfAvailable(_p2H2, false);
                                SetVisibleIfAvailable(_p3H2, false);
                                break;
                        case 2:
                                SetVisibleIfAvailable(_h3, false);
                                SetVisibleIfAvailable(_p2H3, false);
                                SetVisibleIfAvailable(_p3H3, false);
                                break;
                }
        }


        private void OnPointsUpdated(int value)
        {
                if(_p1Label != null) _p1Label.text = value.ToString();
                if(_p2Label != null) _p2Label.text = value.ToString();
                if(_p3Label != null) _p3Label.text = value.ToString();
        }
}
