using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClothesToggle : MonoBehaviour
{
    [HideInInspector] private Toggle toggle;

    [Header("Clothes")]
    [SerializeField] private GameObject ClothedModel;
    [SerializeField] private GameObject nakedModel;

    private void Start()
    {
        toggle = GetComponent<Toggle>();

        toggle.onValueChanged.AddListener(delegate
        {
            ChangeClothingState();
        });
    }

    private void ChangeClothingState()
    {
        ClothedModel.SetActive(toggle.isOn);
        nakedModel.SetActive(!toggle.isOn);
    }
}
