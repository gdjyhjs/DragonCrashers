using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
namespace HappyHarvest
{
    /// <summary>
    /// ��Ⱦ�����뵭��������������ʵ�־����ͼ���ͼ��͸���Ƚ���Ч��
    /// </summary>
    public class RendererFader : MonoBehaviour
    {
        // ͸���ȱ仯���ߣ����ƽ�����̵Ĳ�ֵ��ʽ��
        public AnimationCurve curve;
        // �������ʱ�䣨�룩
        public float time = 0.5f;
        // Ҫ���صľ�����Ⱦ��
        public SpriteRenderer RendererToHide;
        // ����͸����ֵ��0-1��
        public float finalAlpha = 0.2f;

        // ͼ���ͼ���ã����뾫����Ⱦ�����ʹ�ã�
        public Tilemap tilemap;

        // ��ʼ��ɫ�����ڱ���ͻָ�͸���ȣ�
        private Color _initialColor;
        // ��ǰ��ɫ������͸�����޸ģ�
        private Color col;

        void Start()
        {
            // �������ߵ�ǰ�����ģʽ
            curve.preWrapMode = WrapMode.Once;
            curve.postWrapMode = WrapMode.ClampForever;

            // �����ʼ��ɫ
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
        /// ����ײ����봥����ʱ����͸���Ƚ���
        /// </summary>
        private void OnTriggerEnter2D(Collider2D col)
        {
            StartCoroutine(AnimCurve(_initialColor.a, finalAlpha));
        }

        /// <summary>
        /// ����ײ���뿪������ʱ�ָ���ʼ͸����
        /// </summary>
        private void OnTriggerExit2D(Collider2D other)
        {
            StartCoroutine(AnimCurve(finalAlpha, _initialColor.a));
        }

        /// <summary>
        /// ���ڶ������ߵ�͸���Ƚ���Э��
        /// </summary>
        /// <param name="initialPosition">��ʼ͸����</param>
        /// <param name="finalPosition">Ŀ��͸����</param>
        private IEnumerator AnimCurve(float initialPosition, float finalPosition)
        {
            float i = 0;
            float rate = 1 / time;
            while (i < 1)
            {
                i += rate * Time.deltaTime;
                // ʹ�ö������߼��㵱ǰ͸����
                var resultValue = Mathf.Lerp(initialPosition, finalPosition, curve.Evaluate(i));
                col.a = resultValue;

                // Ӧ����ɫ����Ӧ��Ⱦ��
                if (tilemap != null)
                    tilemap.color = col;
                if (RendererToHide != null)
                    RendererToHide.color = col;
                yield return null;
            }
        }
    }
}