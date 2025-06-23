using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HappyHarvest
{
    [DefaultExecutionOrder(1000)]
    public class SpawnPoint : MonoBehaviour
    {
        // ���ɵ����������ڱ�ʶ��ͬ������λ�ã�
        public int SpawnIndex;
        // ����������ʱ���ã�ע�ᵽ��Ϸ��������
        private void OnEnable()
        {
            GameManager.Instance.RegisterSpawn(this);
        }
        // ���������ʱ���ã�����Ϸ������ע����
        private void OnDisable()
        {
            GameManager.Instance?.UnregisterSpawn(this);
        }
        // �ڵ�ǰλ���������
        public void SpawnHere()
        {
            var playerTransform = GameManager.Instance.Player.transform;
            // �������λ��Ϊ��ǰ���ɵ�λ��
            playerTransform.position = transform.position;
            // �������������������������������ע��Ŀ��
            if (GameManager.Instance.MainCamera != null)
            {
                GameManager.Instance.MainCamera.Follow = playerTransform;
                GameManager.Instance.MainCamera.LookAt = playerTransform;
                GameManager.Instance.MainCamera.ForceCameraPosition(playerTransform.position, Quaternion.identity);
            }
        }
    }
#if UNITY_EDITOR
    // �Զ���༭�������ڼ�����ɵ�������Ψһ��
    [CustomEditor(typeof(SpawnPoint))]
    public class SpawnPointEditor : Editor
    {
        // ��д�����������߼�
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            // ��ȡ�������������ɵ����
            SpawnPoint[] transitions = GameObject.FindObjectsByType<SpawnPoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            var local = target as SpawnPoint;
            // ��鵱ǰ���ɵ������Ƿ�Ψһ
            foreach (var transition in transitions)
            {
                if (transition == local)
                    continue;
                if (transition.SpawnIndex == local.SpawnIndex)
                {
                    // ��ʾ������ʾ�������ظ�
                    EditorGUILayout.HelpBox(
                    $"���ɵ���������Ψһ���������ѱ� {transition.gameObject.name} ʹ��",
                    MessageType.Error);
                }
            }
        }
    }
#endif
}