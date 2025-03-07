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

        private void Awake()
        {
                _document = GetComponent<UIDocument>();
                GameManager_v2.Instance.OnUISelected.AddListener(OnUISelected);
                GameManager_v2.Instance.OnPointsUpdated.AddListener(OnPointsUpdated);
        }

        private void OnUISelected()
        {
                VisualElement root = _document.rootVisualElement;
                _p1Label = root.Q<Label>("P1_PointLabel");
                _p2Label = root.Q<Label>("P2_PointLabel");
                _p3Label = root.Q<Label>("P3_PointLabel");
        }



        private void OnPointsUpdated()
        {
                if(_p1Label != null) _p1Label.text = PointManager.Instance.CurrentScore.Value.ToString();
                if(_p2Label != null) _p2Label.text = PointManager.Instance.CurrentScore.Value.ToString();
                if(_p3Label != null) _p3Label.text = PointManager.Instance.CurrentScore.Value.ToString();
        }
}
