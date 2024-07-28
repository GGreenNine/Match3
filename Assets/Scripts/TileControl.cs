using System;
using Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileControl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image _image;
    [SerializeField] private Button _button;
    [SerializeField] private Action<TileControl, Vector3> OnTileDragged;
        
    private Vector3 _dragPosition;
    public TileData TileData;

    public void SetupTile(TileData tileData, int[,] colorsMap, Action<TileControl> onClickAction, Action<TileControl, Vector3> onDragAction)
    {
        _button.onClick.RemoveAllListeners();
        TileData = tileData;
        var colorIndex = colorsMap[tileData.X, tileData.Y];
        SetColor(ColorsMapDefinitions.GetColor(colorIndex));
        _button.onClick.AddListener(() => onClickAction(this));
        OnTileDragged = onDragAction;
    }

    public void SetColor(Color color)
    {
        _image.color = color;
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Camera.main != null)
        {
            Vector3 screenPoint = eventData.position;
            screenPoint.z = Camera.main.WorldToScreenPoint(transform.position).z;
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(screenPoint);
            _dragPosition = worldPoint;
            Debug.Log(worldPoint);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnTileDragged?.Invoke(this, _dragPosition);
    }
}