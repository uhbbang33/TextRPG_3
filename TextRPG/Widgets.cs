using System;
using System.Diagnostics.Metrics;
using System.Text;
using System.Xml.Linq;

namespace TextRPG

{
    class Widget
    {
        protected int _x = 0;
        protected int _y = 0;

        protected int _width = 0;
        public int Width { get { return _width; } }

        protected int _height = 0;
        public int Height { get { return _height; } }

        protected int _maxChildrenCount = 0;
        Dictionary<string, Widget> _children;
        protected int ChildrenCount { get { return _children.Count; } }

        public Widget()
        {
            _children = new Dictionary<string, Widget>();
        }

        public Widget(int x, int y)
        {
            SetPosition(x, y);
            _children = new Dictionary<string, Widget>();
        }

        public Widget(int x, int y, int width, int height)
        {
            _children = new Dictionary<string, Widget>();
            SetPosition(x, y);
            SetSize(width, height);
        }

        public void Draw()
        {
            Draw(0, 0);
        }

        // override  후 base.Draw(_x + x, _y + y) 필요!
        virtual protected void Draw(int x, int y)
        {
            foreach (var widget in _children)
            {
                //Console.ForegroundColor = color;
                widget.Value.Draw(x, y);
            }
        }

        // Widget 추가
        virtual protected void AddChild(string name, Widget widget)
        {
            if (_children.Count < _maxChildrenCount)
            {
                _children.Add(name, widget);
            }
        }

        virtual public void SetPosition(int x, int y)
        {
            _x = x;
            _y = y;
        }

        protected T? GetChild<T>(string name) where T : Widget
        {
            T? child = _children.ContainsKey(name) && _children[name] is T ? (T)_children[name] : null;
            return child;
        }

        virtual public void SetSize(int width, int height)
        {
            _width = width;
            _height = height;
        }

        virtual public void Clear()
        {
            _children.Clear();
        }
    }

    class Text : Widget
    {
        public string text = "";
        ConsoleColor color = ConsoleColor.Black;
        public Text() { }

        public Text(int x, int y) : base(x, y) { }

        protected override void Draw(int x, int y)
        {
            Console.ForegroundColor = color;
            Screen.SetCursorPosition(_x + x, _y + y);
            Console.Write(text);
            Console.ForegroundColor = ConsoleColor.Black;
        }

        public void SetColor(ConsoleColor c)
        {
            color = c;
        }
    }

    class Border : Widget
    {
        public Border()
        {
            _width = 10;
            _height = 5;
            _maxChildrenCount = 1;
        }

        public Border(int x, int y) : base(x, y)
        {
            _width = 10;
            _height = 5;
            _maxChildrenCount = 1;
        }

        public Border(int x, int y, int width, int height) : base(x, y, width, height)
        {
            _maxChildrenCount = 1;
        }

        protected override void Draw(int x, int y)
        {
            DrawBoundary(_x + x, _y + y);
        }

        void DrawBoundary(int x, int y)
        {
            for (int i = x + 1; i < x + _width - 1; ++i)
            {
                Screen.SetCursorPosition(i, y);
                Console.Write("━");

                Screen.SetCursorPosition(i, y + _height - 1);
                Console.Write("━");
            }

            for (int i = y + 1; i < y + _height; ++i)
            {
                Screen.SetCursorPosition(x, i);
                Console.Write("┃");

                Screen.SetCursorPosition(x + _width - 1, i);
                Console.Write("┃");
            }

            Screen.SetCursorPosition(x, y);
            Console.Write("┏");

            Screen.SetCursorPosition(x + _width - 1, y);
            Console.Write("┓");

            Screen.SetCursorPosition(x, y + _height - 1);
            Console.Write("┗");

            Screen.SetCursorPosition(x + _width - 1, y + _height - 1);
            Console.Write("┛");
        }
    }

    class TextBlock : Widget
    {
        public TextBlock()
        {
            _maxChildrenCount = 3;

            _width = 10;
            _height = 3;

            AddChild("Boundary", new Border(0, 0, _width, _height));
            AddChild("Text", new Text(2, 1));
        }

        public TextBlock(int x, int y, int width, int height) : base(x, y, width, height)
        {
            _maxChildrenCount = 3;
            AddChild("Boundary", new Border(0, 0, _width, _height));
            AddChild("Text", new Text(2, 1));
        }

        public void SetText(string text)
        {
            GetChild<Text>("Text").text = text;
        }

        protected override void Draw(int x, int y)
        {
            base.Draw(x + _x, y + _y);
        }

        public override void SetSize(int width, int height)
        {
            base.SetSize(width, height);
            if (GetChild<Border>("Boundary") != null)
                GetChild<Border>("Boundary").SetSize(width, height);
        }
    }

    class StatusWidget : Widget
    {
        public StatusWidget(int x, int y) : base(x, y)
        {
            _maxChildrenCount = 17;

            AddChild("Background", new Border(0, 0, 39, 22));
            AddChild("Content", new Border(2, 1, 35, 20));
            AddChild("SkillContent", new Border(39, 0, 39, 22));
            AddChild("LvText", new Text(5, 2));
            AddChild("ExpText", new Text(15, 2));
            AddChild("ClassText", new Text(5, 4));
            AddChild("AtkText", new Text(5, 6));
            AddChild("DefText", new Text(5, 8));
            AddChild("HPText", new Text(5, 10));
            AddChild("MPText", new Text(5, 12));
            AddChild("CritText", new Text(5, 14));
            AddChild("GoldText", new Text(5, 16));
            AddChild("PotionText", new Text(5, 18));
        }

        public void SetPlayer(Player player)
        {
            string eqAtk;
            string eqDef;
            if (player.Atk - player.BaseAtk > 0)
                eqAtk = $"( +{player.Atk - player.BaseAtk} )";
            else
                eqAtk = "";
            if (player.Def - player.BaseDef > 0)
                eqDef = $"( +{player.Def - player.BaseDef} )";
            else
                eqDef = "";
            GetChild<Text>("LvText").text = $"Lv. {player.Lv}";
            GetChild<Text>("ExpText").text = $"[ {player.Exp} / {player.MaxExp} ]";
            GetChild<Text>("ClassText").text = $"{player.Name} ( {player.Class} )";
            GetChild<Text>("AtkText").text = $"공격력 : {player.Atk} {eqAtk}";
            GetChild<Text>("DefText").text = $"방어력 : {player.Def} {eqDef}";
            GetChild<Text>("HPText").text = $" 체력 : {player.Hp} / {player.MaxHp}";
            GetChild<Text>("MPText").text = $" 마나 : {player.MaxMp}";
            GetChild<Text>("CritText").text = $" 치명타 : {(int)(player.Crit * 100)} %";
            GetChild<Text>("GoldText").text = $" 골드 : {player.Gold} G";
        }
    }

    class ResultWidget : Widget
    {
        public ResultWidget(int x, int y, int width, int height) : base(x, y, width, height)
        {
            _maxChildrenCount = 20;

            AddChild("Background", new Border(0, 0, width, height));
            AddChild("Content", new Border(2, 1, width - 4, height - 2));

            AddChild("EXPLabel", new Text(5, 2));
            AddChild("EXPText", new Text(25, 3));

            AddChild("ItemLabel", new Text(5, 4));

            AddChild("GoldLabel", new Text(5, 17));
            AddChild("GoldText", new Text(23, 18));
        }

        protected override void Draw(int x, int y)
        {
            base.Draw(_x + x, _y + y);
        }

        public void SetResult(Reward reward)
        {
            GetChild<Text>("EXPLabel").text = $"경험치 --------------------";
            GetChild<Text>("EXPText").text = $"+ {reward.Exp,3}";

            GetChild<Text>("ItemLabel").text = $"아이템 --------------------";
            ShowItems(reward.Items);

            GetChild<Text>("GoldLabel").text = $"골드 ----------------------";
            GetChild<Text>("GoldText").text = $"+ {reward.Gold,5} G";
        }

        void ShowItems(List<Item> items)
        {
            for (int i = 0; i < items.Count; ++i)
            {
                AddChild($"Item{i}", new Text(5, 5 + i));
                string str = $"{items[i].Name} x 1";

                str = Utility.MatchCharacterLengthToRight(str, 25, 0);
                GetChild<Text>($"Item{i}").text = str;

            }
        }
    }

    class LevelUpWidget : Widget
    {
        public LevelUpWidget(int x, int y, int width, int height) : base(x, y, width, height)
        {
            _maxChildrenCount = 10;

            AddChild("Content", new Border(0, 0, width, height));
            AddChild("SubContent", new Border(2, 9, width - 4, height - 10));
            AddChild("AtkText", new Text(5, 11));
            AddChild("DefText", new Text(5, 13));
            AddChild("RecoveryText", new Text(5, 15));
            SetText();
        }

        protected override void Draw(int x, int y)
        {
            base.Draw(_x + x, _y + y);

            string[] contents = File.ReadAllLines(@"..\..\..\art\LevelUp.txt");
            for (int i = 0; i < contents.Length; ++i)
            {
                Console.SetCursorPosition(_x + x + 8, _y + y + 3);
                Console.Write(contents[i]);
                ++y;
            }
        }

        void SetText()
        {
            GetChild<Text>("AtkText").text = "공격력이 0.5 증가합니다.";
            GetChild<Text>("DefText").text = "방어력이 1.0 증가합니다.";
            GetChild<Text>("RecoveryText").text = "체력을 모두 회복합니다.";
        }
    }

    class ItemSlot : Widget
    {
        public ItemSlot()
        {
            _maxChildrenCount = 5;

            _width = 38;
            _height = 5;

            AddChild("Content", new Border(0, 0, _width, _height));

            AddChild("ItemNameText", new Text(2, 1));
            AddChild("ItemEffectText", new Text(23, 1));
            AddChild("ItemDescriptionText", new Text(2, 3));
            AddChild("ItemPriceText", new Text(30, 3));
        }

        public void SetItem(int index, Item item)
        {
            string text = item.Name;
            if (item.bEquip) text = text.Insert(0, "[E]");
            if (item.type == Item.EType.Potion)//소모품이면 이름 + 개수까지
            {
                GetChild<Text>("ItemNameText").text = $"{index + 1}. {text}[{item.HasCount}]";
            }
            else//아니면 이름만 출력
            {
                GetChild<Text>("ItemNameText").text = $"{index + 1}. {text}";
            }
            GetChild<Text>("ItemEffectText").text = $"| {item.Status} +{item.Value} |";
            GetChild<Text>("ItemDescriptionText").text = $"{item.Description}";
            GetChild<Text>("ItemPriceText").text = $"{item.Price,5}G";
        }

        protected override void Draw(int x, int y)
        {
            base.Draw(x + _x, y + _y);
        }
    }

    class GridBox : Widget
    {
        int _index = 0;
        int _xMargin = 1;
        int _yMargin = 0;
        int _col = 2;
        protected List<Widget> _widgets;

        public GridBox()
        {
            _maxChildrenCount = 10;
            _widgets = new List<Widget>();
        }

        public void SetMargine(int xMargine/* =1 */, int yMargine/* =0 */)
        {
            _xMargin = xMargine;
            _yMargin = yMargine;
        }

        public void SetColomn(int col)
        {
            _col = col;
        }

        public void AddItem(Widget widget)
        {
            AddChild($"Item{_index++}", widget);
            _widgets.Add(widget);
        }

        protected override void Draw(int x, int y)
        {
            for (int i = 0; i < _widgets.Count; ++i)
            {
                _widgets[i].SetPosition(_xMargin * (i % _col + 1) + _widgets[i].Width * (i % _col),
                    _yMargin * (i / _col + 1) + _widgets[i].Height * (i / _col));
            }
            base.Draw(x + _x, y + _y);
        }

        protected void DrawBase(int x, int y)
        {
            base.Draw(x + _x, y + _y);
        }

        override public void Clear()
        {
            base.Clear();
            _widgets.Clear();
            _index = 0;
        }
    }

    class MonsterGridBox : GridBox
    {
        protected override void Draw(int x, int y)
        {
            for (int i = 0; i < _widgets.Count; ++i)
            {
                if (i == 0) _widgets[i].SetPosition(14, 5);
                else if (i == 1) _widgets[i].SetPosition(50, 5);
                else if (i == 2) _widgets[i].SetPosition(14, 15);
                else if (i == 3) _widgets[i].SetPosition(50, 15);
            }
            DrawBase(x, y);
        }
    }

    class ShopInformationDeskWidget : Widget
    {
        public ShopInformationDeskWidget(int x, int y) : base(x, y)
        {
            _maxChildrenCount = 7;

            AddChild("Content", new Border(0, 0, 40, 9));
            AddChild("Text1", new Text(2, 1));
            AddChild("Text2", new Text(2, 2));
            AddChild("Text3", new Text(2, 4));
            AddChild("Text4", new Text(2, 5));
            AddChild("Text5", new Text(2, 6));
            AddChild("Text6", new Text(2, 7));

            Init();
        }

        void Init()
        {
            GetChild<Text>("Text1").text = "어서오세요.";
            GetChild<Text>("Text2").text = "[일반 상점] 입니다.";
            GetChild<Text>("Text3").text = "무엇을 도와드릴까요?";
            GetChild<Text>("Text4").text = "1. 구입";
            GetChild<Text>("Text5").text = "2. 판매";
            GetChild<Text>("Text6").text = "3. 퀘스트";
        }

        

        protected override void Draw(int x, int y)
        {
            base.Draw(x + _x, y + _y);
        }
    }

    class ShopQuestInfoWidget : Widget
    {
        string _name = "";
        int _num = 0;
        int _reward = 0;

        public ShopQuestInfoWidget(int x, int y, Quest quest) : base(x, y)
        {
            _maxChildrenCount = 7;

            AddChild("Content", new Border(0, 0, 40, 9));
            AddChild("Text1", new Text(2, 1));
            AddChild("Text2", new Text(2, 2));
            AddChild("Text3", new Text(2, 3));
            AddChild("Text4", new Text(2, 5));
            AddChild("Text5", new Text(2, 6));
            AddChild("Text6", new Text(2, 7));

            _name = quest.Name;
            _num = quest.Num;
            _reward = quest.Reward;

            Init();
        }

        public void Init()
        {
            GetChild<Text>("Text1").text = "마침 잘 오셨어요.";
            GetChild<Text>("Text2").text = $"지금 [{_name}] [{_num}개]가 필요한데";
            GetChild<Text>("Text3").text = "구해다 주실 수 있나요? ";
            GetChild<Text>("Text4").text = $"보상: [{_reward} Gold]";
            GetChild<Text>("Text5").text = "1. 수락";
            GetChild<Text>("Text6").text = "2. 거절";
        }

        public void AcceptText()
        {
            GetChild<Text>("Text1").text = "정말 감사해요.";
            GetChild<Text>("Text2").text = $"[{_name}] [{_num}개]를 모으시면";
            GetChild<Text>("Text3").text = "말을 걸어주세요.";
            GetChild<Text>("Text4").text = "";
            GetChild<Text>("Text5").text = "";
            GetChild<Text>("Text6").text = "";
        }

        public void RefuseText()
        {
            GetChild<Text>("Text1").text = "아쉽네요.";
            GetChild<Text>("Text2").text = "마음이 바뀌면";
            GetChild<Text>("Text3").text = "말을 걸어주세요.";
            GetChild<Text>("Text4").text = "";
            GetChild<Text>("Text5").text = "";
            GetChild<Text>("Text6").text = "";
        }

        public void CompletedText()
        {
            GetChild<Text>("Text1").text = "정말 감사해요.";
            GetChild<Text>("Text2").text = "소정의 보상을 드릴게요.";
            GetChild<Text>("Text3").text = "";
            GetChild<Text>("Text4").text = "나중에 또 도와주세요.";
            GetChild<Text>("Text5").text = "";
            GetChild<Text>("Text6").text = $"보상: [{_reward} Gold]";
        }
        public void IncompletedText()
        {
            GetChild<Text>("Text1").text = "말씀 드린 재료를";
            GetChild<Text>("Text2").text = "아직 다 모으지 못하신 것 같아요.";
            GetChild<Text>("Text3").text = "";
            GetChild<Text>("Text4").text = $"[{_name}] [{_num}개]를 다 모으시면";
            GetChild<Text>("Text5").text = "말을 걸어주세요.";
            GetChild<Text>("Text6").text = "";
        }

        protected override void Draw(int x, int y)
        {
            base.Draw(x + _x, y + _y);
        }
    }

    class TempleQuestInfoWidget : Widget
    {
        string _name = "";
        int _num = 0;
        int _reward = 0;

        public TempleQuestInfoWidget(int x, int y, Quest quest) : base(x, y)
        {
            _maxChildrenCount = 11;

            AddChild("Content", new Border(0, 0, 40, 9));
            AddChild("Text1", new Text(2, 1));
            AddChild("Text2", new Text(2, 3));
            AddChild("Text3", new Text(2, 4));
            AddChild("Text4", new Text(2, 5));
            AddChild("Text5", new Text(2, 6));
            AddChild("Text6", new Text(2, 7));
            AddChild("Text7", new Text(2, 8));
            AddChild("Text8", new Text(2, 9));
            AddChild("Text9", new Text(15, 11));

            _name = quest.Name;
            _num = quest.Num;
            _reward = quest.Reward;

            Init();
        }

        public void Init()
        {
            GetChild<Text>("Text1").text = "\t\t공고문";
            GetChild<Text>("Text2").text = $"현재 [{_name}]의";
            GetChild<Text>("Text3").text = "개체수가 너무 많아";
            GetChild<Text>("Text4").text = "피해가 속출하고 있으니";
            GetChild<Text>("Text5").text = $"[{_name}]";
            GetChild<Text>("Text6").text = $"[{_num}마리]를 잡아오면";
            GetChild<Text>("Text7").text = $"[{_reward} Gold]를";
            GetChild<Text>("Text8").text = "보상으로 지급하겠다.";
            GetChild<Text>("Text9").text = "";
        }

        public void AcceptQuest()
        {
            GetChild<Text>("Text9").text = "[진행중]";
        }

        public void RefuseQuest()
        {
            GetChild<Text>("Text9").text = "";
        }

        protected override void Draw(int x, int y)
        {
            base.Draw(x + _x, y + _y);
        }

        public override void SetSize(int width, int height)
        {
            base.SetSize(width, height);
            if (GetChild<Border>("Content") != null)
            {
                GetChild<Border>("Content").SetSize(width, height);
            }
        }
    }

    class UnitViewer : Widget
    {
        public UnitViewer() : base()
        {
            _maxChildrenCount = 10;

            AddChild("Content", new Border(0, 0, 30, 5));
            AddChild("NameText", new Text(2, 1));
            AddChild("LvText", new Text(10, 1));
            AddChild("HPText", new Text(10, 2));
            GetChild<Text>("HPText").SetColor(ConsoleColor.Red);
        }

        public UnitViewer(int x, int y) : base(x, y)
        {
            _maxChildrenCount = 10;

            AddChild("Content", new Border(0, 0, 30, 5));
            AddChild("NameText", new Text(2, 1));
            AddChild("LvText", new Text(10, 1));
            AddChild("HPText", new Text(2, 2));
            GetChild<Text>("HPText").SetColor(ConsoleColor.Red);
        }

        public UnitViewer(int x, int y, int width, int height) : base(x, y, width, height)
        {
            _maxChildrenCount = 10;

            AddChild("Content", new Border(0, 0, 30, 5));
            AddChild("NameText", new Text(2, 1));
            AddChild("LvText", new Text(10, 1));
            AddChild("HPText", new Text(2, 2));
            GetChild<Text>("HPText").SetColor(ConsoleColor.Red);
        }

        public void SetText(string monsterName, int monsterHP, int monsterLv)
        {
            GetChild<Text>("NameText").text = monsterName;
            if (monsterHP <= 0)
            {
                GetChild<Text>("NameText").SetColor(ConsoleColor.Gray);
                GetChild<Text>("HPText").SetColor(ConsoleColor.Gray);
                GetChild<Text>("HPText").text = "Dead";
            }
            else
            {
                GetChild<Text>("HPText").text = monsterHP.ToString();
                GetChild<Text>("LvText").text = $"Lv. {monsterLv,3}";
            }
        }

        protected override void Draw(int x, int y)
        {
            base.Draw(_x + x, _y + y);
        }

        public override void SetSize(int width, int height)
        {
            base.SetSize(width, height);
            if (GetChild<Border>("Content") != null)
            {
                GetChild<Border>("Content").SetSize(width, height);
            }
        }
    }

    class BattleWidget : Widget
    {
        public BattleWidget(int x, int y, int width, int height) : base(x, y, width, height)
        {
            _maxChildrenCount = 4;

            AddChild("Content", new Border(0, 0, width, height));
            AddChild("Text1", new Text(2, 1));
            AddChild("Text2", new Text(2, 2));
            AddChild("Text3", new Text(2, 3));
        }

        public void SetText(string main, string mid, string sub)
        {
            GetChild<Text>("Text1").text = main;
            GetChild<Text>("Text2").text = mid;
            GetChild<Text>("Text3").text = sub;
        }

        protected override void Draw(int x, int y)
        {
            base.Draw(_x + x, _y + y);
        }
    }

    class MessageBox : Widget
    {
        public MessageBox(int x, int y, int width, int height = 3) : base(x, y, width, height)
        {
            _maxChildrenCount = 3;

            AddChild("Background", new Border(0, 0, width, height));
            AddChild("Content", new Border(2, 1, width - 4, height - 2));
            AddChild("MsgText", new Text(5, 2));
        }

        public void SetText(string text)
        {
            GetChild<Text>("MsgText").text = text;
        }

        protected override void Draw(int x, int y)
        {
            base.Draw(x + _x, y + _y);
        }
    }

    class SkillSlot : Widget
    {
        public SkillSlot()
        {
            _maxChildrenCount = 5;
            _width = 38;
            _height = 5;

            AddChild("Content", new Border(0, 0, _width, _height));
            AddChild("NameText", new Text(2, 1));
            AddChild("AtkText", new Text(2, 3));
            AddChild("Accuracy", new Text(23, 3));
        }

        public void SetSkill(int idx, Skill skill)
        {
            GetChild<Text>("NameText").text = $"{idx}. [ {skill.name} ]";
            GetChild<Text>("AtkText").text = $"피해배율 : {skill.damage * 100} %";
            GetChild<Text>("Accuracy").text = $"명중률 : {skill.accuracy * 100} %";
        }

        protected override void Draw(int x, int y)
        {
            base.Draw(_x + x, _y + y);
        }
    }

    public static class Screen
    {
        static int Width;
        static int Height;
        static int Left;
        static int Right;
        static int Boundary = 25;

        public static void SetSize(int width, int height)
        {
            Width = width;
            Height = height;
            Left = 4;
            Right = Left + Width;
            Console.SetWindowSize(Width + 10, Height + 10);

            DrawBoundary();

        }

        public static void Clear()
        {
            Console.Clear();
            DrawBoundary();
        }

        static void DrawBoundary()
        {
            for (int i = Left + 1; i < Right - 1; ++i)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write("━");

                Console.SetCursorPosition(i, Height);
                Console.Write("━");
            }

            for (int i = 1; i < Height; ++i)
            {
                Console.SetCursorPosition(Left, i);
                Console.Write("┃");

                Console.SetCursorPosition(Right, i);
                Console.Write("┃");
            }

            Console.SetCursorPosition(Left, 0);
            Console.Write("┏");

            Console.SetCursorPosition(Right, 0);
            Console.Write("┓");

            Console.SetCursorPosition(Left, Height);
            Console.Write("┗");

            Console.SetCursorPosition(Right, Height);
            Console.Write("┛");
        }

        public static void Split(int yOffset = 0)
        {
            int line = Boundary + yOffset;

            for (int i = Left + 1; i < Right - 1; ++i)
            {
                Console.SetCursorPosition(i, line);
                Console.Write("━");
            }

            Console.SetCursorPosition(Left, line);
            Console.Write("┠");

            Console.SetCursorPosition(Right, line);
            Console.Write("┫");
        }

        public static void ShowMapName(string name, string comment = "") // 색상 설정?
        {
            DeleteLine(1);

            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append($"{name}");
            sb.Append("]");
            sb.Append($" {comment}");

            Console.SetCursorPosition(Left + 3, 1);
            Console.Write(sb.ToString());
        }

        static void DeleteLine(int line)
        {
            for (int i = Left + 1; i < Right; ++i)
            {
                Console.SetCursorPosition(i, line);
                Console.Write(' ');
            }
        }

        public static void DrawScreen(string[] contents, int xOffset, int yOffset)
        {
            Console.Clear();
            DrawBoundary();
            int y = yOffset;
            for (int i = 0; i < contents.Length; ++i)
            {
                Console.SetCursorPosition(Left + xOffset, y);
                Console.Write(contents[i]);
                ++y;
            }
        }

        public static void DrawTopScreen(string[] contents, int xOffset = 3, bool space = false)
        {
            ClearTopScreen();
            int y = 2;

            for (int i = 0; i < contents.Length; ++i)
            {
                if (space) y++;
                Console.SetCursorPosition(Left + xOffset, y++);
                Console.Write(contents[i]);
            }
        }

        public static void DrawBotScreen(string[] contents, int xOffset = 3, bool space = false)
        {
            ClearBotScreen();
            int y = Boundary + 1;

            for (int i = 0; i < contents.Length; ++i)
            {
                if (space) y++;
                Console.SetCursorPosition(Left + xOffset, y++);
                Console.Write($"{i + 1}. {contents[i]}");
            }

            Console.SetCursorPosition(Left + xOffset, Height - 2);
            Console.Write("0. 뒤로가기 / 나가기");
        }

        static void ClearTopScreen()
        {
            for (int i = 2; i < Boundary; ++i)
            {
                DeleteLine(i);
            }
        }

        static void ClearBotScreen()
        {
            for (int i = Boundary + 1; i < Height; ++i)
            {
                DeleteLine(i);
            }
        }

        public static void PrintLine(int xOffset, int y, string line)
        {
            Console.SetCursorPosition(Left + xOffset, y);
            Console.Write(line);
        }

        public static void SetCursorPosition(int x, int y)
        {
            Console.SetCursorPosition(Left + 1 + x, y + 2);
        }
    }
}
