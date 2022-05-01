using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class ClothesToggle : MonoBehaviour
{
    [HideInInspector] private Toggle toggle;
    [HideInInspector] private Transform playerModel;
    [HideInInspector] private GameObject clothedModel;
    [HideInInspector] private GameObject nakedModel;

    [Header("Clothes")]
    [SerializeField] private string clothesPart;

    private void Start()
    {
        toggle = GetComponent<Toggle>();
        playerModel = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().transform.Find("Player Model");
        clothedModel = playerModel.Find("Clothed " + clothesPart).gameObject;
        nakedModel = playerModel.Find("Naked " + clothesPart).gameObject;

        toggle.onValueChanged.AddListener(delegate
        {
            ChangeClothingState();
        });
    }

    private void ChangeClothingState()
    {
        clothedModel.SetActive(toggle.isOn);
        nakedModel.SetActive(!toggle.isOn);
    }
}
