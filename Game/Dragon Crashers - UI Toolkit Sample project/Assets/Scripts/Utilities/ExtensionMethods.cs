using UnityEngine;
using UnityEngine.UIElements;
using System;

/// <summary>
/// 静态类，用于存放扩展方法
/// </summary>
public static class ExtensionMethods
{
    /// <summary>
    /// 将Transform的位置、旋转和缩放重置为默认值。
    /// </summary>
    /// <param name="trans">要重置的Transform。</param>
    public static void ResetTransformation(this Transform trans)
    {
        trans.position = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = new Vector3(1, 1, 1);
    }

    /// <summary>
    /// 返回VisualElement中心的世界空间位置。
    /// </summary>
    /// <param name="visualElement">要计算中心位置的VisualElement。</param>
    /// <param name="camera">用于转换的相机（可选，默认为Camera.main）。</param>
    /// <param name="zDepth">元素在世界空间中的Z深度（默认值为10）。</param>
    /// <returns>VisualElement中心的世界空间位置。</returns>
    public static Vector3 GetWorldPosition(this VisualElement visualElement, Camera camera = null, float zDepth = 10f)
    {
        if (camera == null)
            camera = Camera.main;

        Vector3 worldPos = Vector3.zero;

        if (camera == null || visualElement == null)
            return worldPos;

        return visualElement.worldBound.center.ScreenPosToWorldPos(camera, zDepth);
    }

    /// <summary>
    /// 将UI Toolkit的屏幕位置转换为世界空间坐标。
    /// </summary>
    /// <param name="screenPos">要转换的屏幕位置。</param>
    /// <param name="camera">用于转换的相机（可选，默认为Camera.main）。</param>
    /// <param name="zDepth">世界空间位置的Z深度（默认值为10）。</param>
    /// <returns>与屏幕位置对应的世界空间位置。</returns>
    public static Vector3 ScreenPosToWorldPos(this Vector2 screenPos, Camera camera = null, float zDepth = 10f)
    {
        if (camera == null)
            camera = Camera.main;

        if (camera == null)
            return Vector2.zero;

        float xPos = screenPos.x;
        float yPos = screenPos.y;
        Vector3 worldPos = Vector3.zero;

        if (!float.IsNaN(screenPos.x) && !float.IsNaN(screenPos.y) && !float.IsInfinity(screenPos.x) &&
            !float.IsInfinity(screenPos.y))
        {
            // 使用Camera类将其转换为世界空间位置
            Vector3 screenCoord = new Vector3(xPos, yPos, zDepth);
            worldPos = camera.ScreenToWorldPoint(screenCoord);
        }

        return worldPos;
    }

    /// <summary>
    /// 将UI Toolkit的点击事件位置转换为像素级的屏幕坐标。
    /// </summary>
    /// <param name="clickPosition">要转换的点击事件位置。</param>
    /// <param name="rootVisualElement">UI层次结构的根VisualElement。</param>
    /// <returns>像素级的屏幕坐标。</returns>
    public static Vector2 GetScreenCoordinate(this Vector2 clickPosition, VisualElement rootVisualElement)
    {
        // 调整点击位置以适应边框（用于安全区域边框）
        float borderLeft = rootVisualElement.resolvedStyle.borderLeftWidth;
        float borderTop = rootVisualElement.resolvedStyle.borderTopWidth;
        clickPosition.x += borderLeft;
        clickPosition.y += borderTop;

        // 归一化UI Toolkit位置以考虑面板匹配设置
        Vector2 normalizedPosition = clickPosition.NormalizeClickEventPosition(rootVisualElement);

        // 乘以屏幕尺寸以获得像素级的屏幕坐标
        float xValue = normalizedPosition.x * Screen.width;
        float yValue = normalizedPosition.y * Screen.height;
        return new Vector2(xValue, yValue);
    }

    /// <summary>
    /// 将点击事件位置归一化为根VisualElement内的(0, 0)到(1, 1)范围。
    /// </summary>
    /// <param name="clickPosition">要归一化的点击事件位置。</param>
    /// <param name="rootVisualElement">用于归一化参考的根VisualElement。</param>
    /// <returns>归一化后的位置，范围为(0,0)到(1,1)。</returns>
    public static Vector2 NormalizeClickEventPosition(this Vector2 clickPosition, VisualElement rootVisualElement)
    {
        // 获取代表UI Toolkit中屏幕边界的矩形
        Rect rootWorldBound = rootVisualElement.worldBound;

        float normalizedX = clickPosition.x / rootWorldBound.xMax;

        // 翻转y值，使y = 0位于屏幕底部
        float normalizedY = 1 - clickPosition.y / rootWorldBound.yMax;

        return new Vector2(normalizedX, normalizedY);
    }

    /// <summary>
    /// 将VisualElement对齐到指定的世界位置。
    /// </summary>
    /// <param name="element">要移动的VisualElement。</param>
    /// <param name="worldPosition">目标世界位置。</param>
    /// <param name="worldSize">要对齐的世界对象的大小。</param>
    public static void MoveToWorldPosition(this VisualElement element, Vector3 worldPosition, Vector2 worldSize)
    {
        Rect rect = RuntimePanelUtils.CameraTransformWorldToPanelRect(element.panel, worldPosition, worldSize,
            Camera.main);
        element.transform.position = rect.position;
    }

    /// <summary>
    /// 使VisualElement保持在相机视口内。
    /// </summary>
    /// <param name="element">要重新定位的元素。</param>
    /// <param name="camera">观察相机。</param>
    public static void ClampToScreenBounds(this VisualElement element, Camera camera = null)
    {
        camera ??= Camera.main;
        if (camera == null || element == null)
            return;

        // 计算整个层次结构的边界矩形
        Rect boundingRect = new Rect(element.worldBound.position, element.worldBound.size);

        // 扩展边界以包含任何子元素
        foreach (VisualElement child in element.Children())
        {
            Rect childRect = child.worldBound;
            boundingRect.xMin = Mathf.Min(boundingRect.xMin, childRect.xMin);
            boundingRect.xMax = Mathf.Max(boundingRect.xMax, childRect.xMax);
            boundingRect.yMin = Mathf.Min(boundingRect.yMin, childRect.yMin);
            boundingRect.yMax = Mathf.Max(boundingRect.yMax, childRect.yMax);
        }

        Vector3 viewportPosition = camera.WorldToViewportPoint(boundingRect.center);

        // 限制在屏幕空间内，考虑边界矩形的尺寸
        viewportPosition.x = Mathf.Clamp(viewportPosition.x, boundingRect.width / 2 / Screen.width,
            1 - boundingRect.width / 2 / Screen.width);
        viewportPosition.y = Mathf.Clamp(viewportPosition.y, boundingRect.height / 2 / Screen.height,
            1 - boundingRect.height / 2 / Screen.height);

        // 转换回世界位置并设置
        Vector3 newWorldPosition = camera.ViewportToWorldPoint(viewportPosition);

        Vector3 offset = newWorldPosition -
                         new Vector3(boundingRect.center.x, boundingRect.center.y, newWorldPosition.z);

        element.transform.position += offset;
    }

    /// <summary>
    /// 根据VisualElement的宽度设置其高度，以保持给定的宽高比。
    /// </summary>
    /// <param name="element">要调整的VisualElement。</param>
    /// <param name="aspectRatio">所需的宽高比（例如，1.0表示正方形）。</param>
    public static void SetAspectRatio(this VisualElement element, float aspectRatio)
    {
        // 注册元素几何形状更改时的回调（例如，调整大小）
        element.RegisterCallback<GeometryChangedEvent>(evt =>
        {
            // 设置高度为宽度除以宽高比
            element.style.height = element.resolvedStyle.width / aspectRatio;
        });
    }

    /// <summary>
    /// 将VisualElement对齐到指定GameObject的屏幕位置，使元素的中心位于对象的位置。
    /// </summary>
    /// <param name="element">要定位在GameObject上方的VisualElement。</param>
    /// <param name="gameObject">VisualElement应对齐的GameObject。</param>
    /// <param name="camera">用于捕获GameObject在屏幕坐标中位置的相机。</param>
    /// <param name="rootElement">包含UI布局的根VisualElement，用于坐标转换。</param>
    public static void AlignToGameObject(this VisualElement element, GameObject gameObject, Camera camera,
        VisualElement rootElement)
    {
        if (gameObject == null || camera == null || rootElement == null)
            return;

        // 获取GameObject的世界位置并将其转换为屏幕坐标
        Vector3 worldPos = gameObject.transform.position;
        Vector3 screenPos = camera.WorldToScreenPoint(worldPos);

        // 将屏幕位置转换为根元素的局部位置
        Vector2 localPos = ScreenToLocal(rootElement, screenPos);

        // 居中元素
        element.style.left = localPos.x - (element.resolvedStyle.width / 2);
        element.style.top = localPos.y - (element.resolvedStyle.height / 2);
    }

    /// <summary>
    /// 将屏幕坐标转换为根VisualElement内的局部坐标。
    /// </summary>
    /// <param name="rootElement">目标根VisualElement。</param>
    /// <param name="screenPos">要转换的屏幕位置。</param>
    /// <returns>根元素内的局部位置。</returns>
    private static Vector2 ScreenToLocal(VisualElement rootElement, Vector3 screenPos)
    {
        // 反转Y坐标；屏幕坐标的原点在左下角，而UI的原点在左上角
        screenPos.y = Screen.height - screenPos.y;

        // 将屏幕位置转换为根元素内的局部位置
        return rootElement.WorldToLocal(screenPos);
    }

    /// <summary>
    /// 使用新的本地化字符串更新DropdownField的选项，同时保存当前选择。
    /// DropdownFields仅存储显示文本，而不存储值，因此我们使用数组来维护有序选项。
    /// </summary>
    /// <param name="dropdown">要更新的下拉菜单</param>
    /// <param name="localizedChoices">要显示的新本地化字符串数组</param>
    /// <param name="currentValue">要保留的当前内部值。</param>
    /// <param name="optionKeys">映射到选项的内部值数组。</param>
    public static void UpdateLocalizedChoices(this DropdownField dropdown, string[] localizedChoices,
        string currentValue, string[] optionKeys)
    {
        if (dropdown == null)
            return;

        // 用新的本地化选项替换现有的选项
        dropdown.choices.Clear();
        dropdown.choices.AddRange(localizedChoices);

        // 在optionKeys数组中找到当前值的索引
        int savedIndex = Array.IndexOf(optionKeys, currentValue);
        if (savedIndex >= 0 && savedIndex < dropdown.choices.Count)
        {
            dropdown.index = savedIndex;

            // 设置DropdownField的选定值，而不触发任何事件回调
            dropdown.SetValueWithoutNotify(dropdown.choices[savedIndex]);
        }

        // 立即重绘下拉菜单以反映更改，即使没有触发任何事件
        dropdown.MarkDirtyRepaint();
    }
}