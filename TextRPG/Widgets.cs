using System.Diagnostics.Metrics;
using System.Text;

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
            _maxChildrenCount = 11;

            AddChild("Background", new Border(0, 0, 39, 20));

            AddChild("Content", new Border(2, 1, 35, 18));

            AddChild("LvText", new Text(5, 2));
            AddChild("ExpText", new Text(15, 2));
            AddChild("ClassText", new Text(5, 4));
            AddChild("AtkText", new Text(5, 6));
            AddChild("DefText", new Text(5, 8));
            AddChild("HPText", new Text(5, 10));
            AddChild("CritText", new Text(5, 12));
            AddChild("GoldText", new Text(5, 14));
            AddChild("PotionText", new Text(5, 16));
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
            GetChild<Text>("CritText").text = $" 치명타 : {player.Crit} %";
            GetChild<Text>("GoldText").text = $" 골드 : {player.Gold} G";
            GetChild<Text>("PotionText").text = $" 포션 : {player.hasPotion} 개";
        }
    }

    class ResultWidget : Widget
    {
        public ResultWidget(int x, int y, int width, int height) : base(x, y, width, height)
        {
            _maxChildrenCount = 20;

            AddChild("Background", new Border(0, 0, width, height));
            AddChild("Content", new Border(2, 1, width - 4, height - 2));

            //AddChild("LvLabel", new Text(5, 2));
            //AddChild("LvText", new Text(25, 3));

            AddChild("EXPLabel", new Text(5, 2));
            AddChild("EXPText", new Text(25, 5));

            AddChild("ItemLabel", new Text(5, 4));

            //AddChild("HPLabel", new Text(5, 6));
            //AddChild("HPText", new Text(25, 7));

            AddChild("GoldLabel", new Text(5, 17));
            AddChild("GoldText", new Text(23, 18));

            //AddChild("LevelUpText", new Text(5, 11));
            //AddChild("AtkText", new Text(25, 13));
            //AddChild("DefText", new Text(25, 15));
        }

        protected override void Draw(int x, int y)
        {
            base.Draw(_x + x, _y + y);
        }

        public void SetResult(Reward reward)
        {
            GetChild<Text>("EXPLabel").text = $"경험치 --------------------";
            GetChild<Text>("EXPText").text = $"+ {reward.Exp, 3}";

            GetChild<Text>("ItemLabel").text = $"아이템 --------------------";
            ShowItems(reward.Items);

            GetChild<Text>("GoldLabel").text = $"골드 ----------------------";
            GetChild<Text>("GoldText").text = $"+ {reward.Gold,5} G";
        }

        void ShowItems(List<Item> items)
        {
            for(int i = 0; i < items.Count; ++i)
            {
                AddChild($"Item{i}", new Text(5, 5 + i));
                string str = $"{items[i].Name} x 1";
                GetChild<Text>($"Item{i}").text = $"{str, 25}";
            }            
        }

        public void SetResult(Record before, Record after)
        {
            //GetChild<Text>("LvLabel").text = $"레벨 --------------------------";
            //GetChild<Text>("LvText").text = $"{before.lv,3} --> {after.lv,3}";

            GetChild<Text>("EXPLabel").text = $"경험치 ------------------------";
            GetChild<Text>("EXPText").text = $"{before.exp,3} / {before.maxExp,3} --> {after.exp,3} / {after.maxExp,3}";

            GetChild<Text>("HPLabel").text = $"체력 --------------------------";
            GetChild<Text>("HPText").text = $"{before.hp,3} --> {after.hp,3}";

            GetChild<Text>("GoldLabel").text = $"골드 --------------------------";
            GetChild<Text>("GoldText").text = $"{before.gold,5} G --> {after.gold,5} G";

            if (before.lv != after.lv)
            {
                GetChild<Text>("LevelUpText").text = "LEVEL UP !!";
                GetChild<Text>("AtkText").text = $"공격력 + {2}";
                GetChild<Text>("DefText").text = $"방어력 + {1}";
            }
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
            AddChild("ItemEffectText", new Text(20, 1));
            AddChild("ItemDescriptionText", new Text(2, 3));
            AddChild("ItemPriceText", new Text(30, 3));
        }

        public void SetItem(int index, Item item)
        {
            string text = item.Name;
            if (item.bEquip) text = text.Insert(0, "[E]");
            GetChild<Text>("ItemNameText").text = $"{index + 1}. {text}";
            GetChild<Text>("ItemEffectText").text = $"| {item.Status} + {item.Value} |";
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
                if (i == 0) _widgets[i].SetPosition(14, 6);
                else if( i == 1) _widgets[i].SetPosition(50, 6);
                else if( i == 2) _widgets[i].SetPosition(14, 16);
                else if( i == 3) _widgets[i].SetPosition(50, 16);
            }
            DrawBase(x, y);
        }
    }

    class ShopInformationDeskWidget : Widget
    {
        public ShopInformationDeskWidget(int x, int y) : base(x, y)
        {
            _maxChildrenCount = 6;

            AddChild("Content", new Border(0, 0, 40, 9));
            AddChild("Text1", new Text(2, 1));
            AddChild("Text2", new Text(2, 2));
            AddChild("Text3", new Text(2, 4));
            AddChild("Text4", new Text(2, 5));
            AddChild("Text5", new Text(2, 6));

            Init();
        }

        void Init()
        {
            GetChild<Text>("Text1").text = "어서오세요.";
            GetChild<Text>("Text2").text = "[일반 상점] 입니다.";
            GetChild<Text>("Text3").text = "무엇을 도와드릴까요?";
            GetChild<Text>("Text4").text = "1. 구입";
            GetChild<Text>("Text5").text = "2. 판매";
        }

        protected override void Draw(int x, int y)
        {
            base.Draw(x + _x, y + _y);
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
                GetChild<Text>("LvText").text = $"Lv. {monsterLv, 3}";
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
