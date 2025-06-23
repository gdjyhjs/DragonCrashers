using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
namespace HappyHarvest
{
    /// <summary>
    /// 渲染器淡入淡出控制器，用于实现精灵或图块地图的透明度渐变效果
    /// </summary>
    public class RendererFader : MonoBehaviour
    {
        // 透明度变化曲线（控制渐变过程的插值方式）
        public AnimationCurve curve;
        // 渐变持续时间（秒）
        public float time = 0.5f;
        // 要隐藏的精灵渲染器
        public SpriteRenderer RendererToHide;
        // 最终透明度值（0-1）
        public float finalAlpha = 0.2f;

        // 图块地图引用（可与精灵渲染器配合使用）
        public Tilemap tilemap;

        // 初始颜色（用于保存和恢复透明度）
        private Color _initialColor;
        // 当前颜色（用于透明度修改）
        private Color col;

        void Start()
        {
            // 设置曲线的前后包裹模式
            curve.preWrapMode = WrapMode.Once;
            curve.postWrapMode = WrapMode.ClampForever;

            // 保存初始颜色
            if (RendererToHide != null)
            {
                _initialColor = RendererToHide.color;
            }

            if (tilemap != null)
            {
                _initialColor = tilemap.color;
            }

            col = _initialColor;
        }

        /// <summary>
        /// 当碰撞体进入触发器时触发透明度渐变
        /// </summary>
        private void OnTriggerEnter2D(Collider2D col)
        {
            StartCoroutine(AnimCurve(_initialColor.a, finalAlpha));
        }

        /// <summary>
        /// 当碰撞体离开触发器时恢复初始透明度
        /// </summary>
        private void OnTriggerExit2D(Collider2D other)
        {
            StartCoroutine(AnimCurve(finalAlpha, _initialColor.a));
        }

        /// <summary>
        /// 基于动画曲线的透明度渐变协程
        /// </summary>
        /// <param name="initialPosition">初始透明度</param>
        /// <param name="finalPosition">目标透明度</param>
        private IEnumerator AnimCurve(float initialPosition, float finalPosition)
        {
            float i = 0;
            float rate = 1 / time;
            while (i < 1)
            {
                i += rate * Time.deltaTime;
                // 使用动画曲线计算当前透明度
                var resultValue = Mathf.Lerp(initialPosition, finalPosition, curve.Evaluate(i));
                col.a = resultValue;

                // 应用颜色到对应渲染器
                if (tilemap != null)
                    tilemap.color = col;
                if (RendererToHide != null)
                    RendererToHide.color = col;
                yield return null;
            }
        }
    }
}