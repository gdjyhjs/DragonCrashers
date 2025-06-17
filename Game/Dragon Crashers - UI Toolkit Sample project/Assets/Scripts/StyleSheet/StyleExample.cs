// 样式指南 - UI工具包演示

// 命名/大小写：
// - 除非另有说明，否则使用帕斯卡命名法（例如ExamplePlayerController、MaxHealth等）
// - 局部/私有变量、参数使用驼峰命名法（例如examplePlayerController、maxHealth等）
// - USS类使用短横线命名法（例如char-data-panel、char-data-tabs、char-data-content等）
// - UXML文件使用帕斯卡命名法（例如CharScreen.uxml、HomeScreen.uxml等）
// - 私有成员变量使用m_前缀（例如m_PlayerHealth、m_ShopScreen）
// - 项目特定脚本使用UIToolkitDemo命名空间

// 格式：
// - 使用Allman风格的大括号（开括号另起一行）。
// - 保持每行较短（80 - 120个字符）。 
// - 在流控制条件前使用单个空格，例如while (x == y)
// - 避免在方括号内使用空格，例如x = dataArray[index]
// - 在函数参数的逗号后使用单个空格。
// - 不要在括号和函数参数后添加空格，例如CollectItem(myObject, 0, 1);
// - 不要在函数名和括号之间使用空格，例如DropPowerUp(myPrefab, 0, 1);
// - 避免使用区域。

// 注释：
// - 使用//注释将解释与逻辑放在一起。
// - 对于序列化字段，使用工具提示而不是注释。 

// 使用语句：
// - 将使用语句放在文件顶部。
// - 移除未使用的语句。
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 命名空间：
// - 使用帕斯卡命名法，不使用特殊符号或下划线。
// - 在顶部添加使用语句以避免重复输入命名空间。
// - 使用点（.）运算符创建子命名空间，例如MyApplication.GameFlow、MyApplication.AI等。
namespace StyleSheetExample
{

    // 枚举：
    // - 使用单数类型名称。
    // - 不使用前缀或后缀。
    // - 使用帕斯卡命名法
    public enum Rarity
    {
        // 稀有
        Rare,
        // 特殊
        Special,
        // 普通
        Common
    }

    // 标志枚举：
    // - 使用复数类型名称 
    // - 不使用前缀或后缀。
    // - 对二进制值使用列对齐
    [Flags]
    public enum AttackModes
    {
        // 十进制                         // 二进制
        // 无
        None = 0,                          // 000000
        // 近战
        Melee = 1,                         // 000001
        // 远程
        Ranged = 2,                        // 000010
        // 特殊
        Special = 4,                       // 000100

        // 近战和特殊
        MeleeAndSpecial = Melee | Special  // 000101
    }

    // 接口：
    // - 用形容词短语命名接口。
    // - 使用'I'前缀。
    public interface IDamageable
    {
        string damageTypeName { get; }
        float damageValue { get; }

        // 方法：
        // - 方法名以动词或动词短语开头，以表示一个动作。
        // - 参数名使用驼峰命名法。
        bool ApplyDamage(string description, float damage, int numberOfHits);
    }

    public interface IDamageable<T>
    {
        void Damage(T damageTaken);
    }

    // 类或结构体：
    // - 用名词或名词短语命名。
    // - 避免使用前缀。
    // - 每个文件一个MonoBehaviour。如果文件中有MonoBehaviour，则源文件名必须匹配。 
    public class StyleExample : MonoBehaviour
    {

        // 字段： 
        // - 避免使用特殊字符（反斜杠、符号、Unicode字符）
        // - 使用名词作为名称，但布尔值前缀使用动词。
        // - 使用有意义的名称。使名称可搜索且易读。不要缩写（除非是数学相关）。
        // - 公共字段使用帕斯卡命名法。私有变量使用驼峰命名法。
        // - 在私有字段前添加下划线（m_）以区别于局部变量（不使用其他前缀）
        // - 省略默认的私有访问修饰符

        // 经过的天数
        int m_ElapsedTimeInDays;

        // 如果要在检查器中显示私有字段，请使用[SerializeField]属性。
        // 布尔值应提出一个可以用true或false回答的问题。
        // 是否玩家已死亡
        [SerializeField] bool m_IsPlayerDead;

        // 这将值限制在一个范围内，并在检查器中创建一个滑块。
        // 远程统计
        [Range(0f, 1f)] [SerializeField] float m_RangedStat;

        // 工具提示可以替代序列化字段上的注释，并起到双重作用。
        // 另一个统计
        [Tooltip("这是玩家的另一个统计数据。")]
        [SerializeField] float m_AnotherStat;


        // 属性：
        // - 优于公共字段。
        // - 使用帕斯卡命名法，不使用特殊字符。
        // - 使用表达式体属性以缩短代码
        // - 例如，只读属性使用表达式体，但其他情况使用{ get; set; }。
        // - 对于没有后备字段的公共属性，使用自动实现属性。

        // 私有后备字段
        int m_MaxHealth;

        // 只读，返回后备字段（最常见）
        public int MaxHealthReadOnly => m_MaxHealth;

        // 显式实现getter和setter
        public int MaxHealth
        {
            get => m_MaxHealth;
            set => m_MaxHealth = value;
        }

        // 只写（不使用后备字段）
        public int Health { private get; set; }

        // 只写，没有显式的setter
        public void SetMaxHealth(int newMaxValue) => m_MaxHealth = newMaxValue;

        // 没有后备字段的自动实现属性
        public string DescriptionName { get; set; } = "火球术";

#pragma warning disable 0067

        // 事件：
        // - 用动词短语命名。
        // - 现在分词表示“之前”，过去分词表示“之后”。
        // - 大多数事件使用System.Action委托（可以接受0到16个参数）。
        // - 仅在必要时定义自定义EventArg（可以是System.EventArgs或自定义结构体）。

        // 开门前事件
        public event Action OpeningDoor;

        // 开门后事件
        public event Action DoorOpened;

        // 得分事件
        public event Action<int> PointsScored;
        // 事件发生事件
        public event Action<CustomEventArgs> ThingHappened;

#pragma warning restore 0067

        // 这些是事件触发方法，例如OnDoorOpened、OnPointsScored等。
        public void OnDoorOpened()
        {
            DoorOpened?.Invoke();
        }

        public void OnPointsScored(int points)
        {
            PointsScored?.Invoke(points);
        }

        // 这是一个由结构体组成的自定义EventArg。
        public struct CustomEventArgs
        {
            public int ObjectID { get; }
            public Color Color { get; }

            public CustomEventArgs(int objectId, Color color)
            {
                this.ObjectID = objectId;
                this.Color = color;
            }
        }

        // 方法：
        // - 方法名以动词或动词短语开头，以表示一个动作。
        // - 参数名使用驼峰命名法。

        // 方法以动词开头。
        // 设置初始位置
        public void SetInitialPosition(float x, float y, float z)
        {
            transform.position = new Vector3(x, y, z);
        }

        // 当方法返回布尔值时，方法应提出一个问题。
        // 是否是新位置
        public bool IsNewPosition(Vector3 newPosition)
        {
            return (transform.position == newPosition);
        }

        private void FormatExamples(int someExpression)
        {
            // var关键字：
            // - 如果有助于提高可读性，尤其是在类型名称较长时，使用var。
            // - 如果会使类型模糊不清，则避免使用var。
            var powerUps = new List<GameObject>();
            var dict = new Dictionary<string, List<GameObject>>();


            // 开关语句：
            switch (someExpression)
            {
                case 0:
                    // ..
                    break;
                case 1:
                    // ..
                    break;
                case 2:
                    // ..
                    break;
            }

            // 大括号： 
            // - 在使用单行语句时，为了清晰起见保留大括号。
            // - 或者为了便于调试，完全避免使用单行语句。
            // - 在嵌套的多行语句中保留大括号。

            // 这个单行语句保留了大括号...
            for (int i = 0; i < 100; i++) { DoSomething(i); }

            // ... 但这样更便于调试。你可以在子句上设置断点。
            for (int i = 0; i < 100; i++)
            {
                DoSomething(i);
            }

            // 这里不要移除大括号。
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    DoSomething(j);
                }
            }
        }

        private void DoSomething(int x)
        {
            // .. 
        }
    }

    // 结构体和ScriptableObject：
    // - 字段使用驼峰命名法
    // - ScriptableObject方法使用帕斯卡命名法
    [System.Serializable]
    public struct RarityIcon
    {
        // 图标
        public Sprite icon;
        // 稀有度
        public Rarity rarity;
    }

    public class GameIconsSO : ScriptableObject
    {
        // 稀有度图标列表
        public List<RarityIcon> rarityIcons;
        // 获取稀有度图标
        public Sprite GetRarityIcon(Rarity rarity)
        {
            RarityIcon match = rarityIcons.Find(x => x.rarity == rarity);
            return match.icon;
        }
    }
}