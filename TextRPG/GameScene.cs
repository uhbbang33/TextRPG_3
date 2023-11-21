using System;
using System.ComponentModel.Design;

namespace TextRPG

{
    class Scene
    {
        // 해당 씬에서 넘어갈 수 있는 다음 씬
        protected static Dictionary<string, Scene> SceneGroup = new Dictionary<string, Scene>();

        // 해당 씬의 이전 씬
        protected Scene _prev;
        public Scene Prev { get { return _prev; } set { _prev = value; } }

        protected string _name = "";
        public string Name { get { return _name; } }

        // 해당 씬의 추가적인 설명, 씬 이름 우측에 표시됨
        protected string _comment = "";

        // 화면 하단에 표출할 문자열 배열
        protected string[] _choices = { };
        public string[] Option { get { return _choices; } }

        // 화면 상단에 표출할 문자열 배열
        protected string[] _display = { };
        public string[] Display { get { return _display; } }

        static MessageBox _board = new MessageBox(30, 33, 43, 5);
        static TextBlock _goldText = new TextBlock(63, 20, 15, 3);
        static TextBlock _pagination = new TextBlock(3, 20, 26, 3);

        protected void AddScene(string name, Scene scene)
        {
            if (SceneGroup.ContainsKey(name) == false)
            {
                SceneGroup.Add(name, scene);
            }
        }

        // 경고 또는 알림 창
        protected void ThrowMessage(string msg)
        {
            _board.SetText(msg);
            _board.Draw();
            Thread.Sleep(1000);
            GameManager.Instance.RefreshScene();
        }

        // 보유 골드 표시
        protected void ShowGold()
        {
            string playerGold = GameManager.Instance.Player.Gold.ToString();
            _goldText.SetText($"{playerGold,10} G");
            _goldText.Draw();
        }

        protected void ShowPagination()
        {
            int currentPage = GameManager.Instance.Player.invenPage + 1;
            int lastPage = GameManager.Instance.Player.Inventory.Count / 6 + 1;
            _pagination.SetText($"◀이전 Q ({currentPage}/{lastPage}) E 다음▶");
            _pagination.Draw();
        }

        protected void ShowPagination(int currentPage, int lastPage)
        {
            _pagination.SetText($"◀이전 Q ({currentPage}/{lastPage}) E 다음▶");
            _pagination.Draw();
        }

        // 입력 처리
        virtual public void HandleInput(GameManager game, ConsoleKey key) { }

        // 게임 씬을 드로우 하기 전에 업데이트 할 내용
        virtual public void Update(GameManager game) { }

        /// <summary>
        /// 기본적으로 맵의 이름과 선택지를 출력
        /// </summary>
        virtual public void DrawScene()
        {
            Screen.ShowMapName(_name, _comment);
            Screen.DrawBotScreen(Option, 3, true);
        }
    }

    class TitleScene : Scene
    {
        public TitleScene()
        {
            _name = "타이틀";

            //넘어갈 선택창 생성
            AddScene("StartOrContinue", new StartOrContinueScene(this));

            _display = File.ReadAllLines(@"..\..\..\art\Title.txt");
        }

        override public void HandleInput(GameManager game, ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.Enter:
                    game.ChangeScene(SceneGroup["StartOrContinue"]);
                    break;
            }
        }

        public override void DrawScene()
        {
            Screen.SetSize(80, 40);
            Screen.DrawScreen(Display, 5, 0);
        }
    }

    class StartOrContinueScene : Scene
    {
        public StartOrContinueScene(Scene parent)
        {
            _name = "시작 화면";
            _comment = "새로 캐릭터를 생성하거나 이어하기";
            _prev = parent;

            //선택 화면에 출력
            _choices = new string[] { "새로하기", "이어하기" };

            //다음으로 넘어갈 TownScene을 딕셔너리에 추가
            AddScene("Town", new TownScene(this));
            //새 캐릭터를 만드는 SelectClassScene을 추가
            AddScene("ClassSelect", new SelectClassScene(this));

            //위 화면에 출력할 아스키아트 로드
            SetDisplay();
        }

        override public void HandleInput(GameManager game, ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.D0:
                    game.ChangeScene(_prev);
                    break;
                case ConsoleKey.D1://새로하기
                    game.ChangeScene(SceneGroup["ClassSelect"]);
                    break;
                case ConsoleKey.D2://이어하기
                    game.ChangeScene(SceneGroup["Town"]);
                    break;
                default:
                    ThrowMessage("잘못된 입력입니다.");
                    break;
            }
        }

        //아스키 아트 로드
        void SetDisplay()
        {
            //_display = File.ReadAllLines(@"..\..\..\art\새그림.txt");
        }


        public override void DrawScene()
        {
            //씬 이름과 설명 출력
            base.DrawScene();

            //선택창을 위해 화면 분할
            Screen.Split();
            //화면 맨 위부터 화면 그리기
            Screen.DrawTopScreen(Display, 2);
        }
    }
    //직업 선택 씬
    class SelectClassScene : Scene
    {
        GridBox classBorders;

        public SelectClassScene(Scene parent)
        {
            _name = "직업 선택";
            _comment = "원하는 직업을 선택하세요";
            _prev = parent;

            //선택 화면에 출력
            _choices = new string[] { "전사", "마법사" };

            //위 화면에 출력할 아스키아트 로드
            SetDisplay();

            classBorders = new GridBox();
            classBorders.SetColomn(2);
            classBorders.SetMargine(2, 0);
            classBorders.SetPosition(2, 1);
            
            for(int i = 0; i < 2; ++i)
            {
                Border border = new Border();
                border.SetSize(35, 22);
                classBorders.AddItem(border);
            }
        }

        override public void HandleInput(GameManager game, ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.D0:
                    game.ChangeScene(_prev);
                    break;
                case ConsoleKey.D1://전사 클래스 선택 시, 플레이어 직업 반영
                    game.Player.SetWarrior(GetName());
                    game.ChangeScene(SceneGroup["Town"]);
                    break;
                case ConsoleKey.D2://마법사 클래스 선택 시, 플레이어 직업 반영
                    game.Player.SetWizard(GetName());
                    game.ChangeScene(SceneGroup["Town"]);
                    break;
                default:
                    ThrowMessage("잘못된 입력입니다.");
                    break;
            }
        }

        //아스키 아트 로드
        void SetDisplay()
        {
            _display = File.ReadAllLines(@"..\..\..\art\Class_art.txt");
        }


        public override void DrawScene()
        {
            //씬 이름과 설명 출력
            base.DrawScene();
            //선택창을 위해 화면 분할
            Screen.Split();
            //화면 맨 위부터 화면 그리기
            Screen.DrawTopScreen(Display, 2);
            classBorders.Draw();
        }

        private string GetName()
        {
            Screen.Clear();

            _display = new string[] { "닉네임을 정해주세요 = " };

            //화면 해당 좌표에 출력
            Screen.DrawScreen(Display, 20, 20);

            //이름 입력 받기
            string name = Console.ReadLine();

            //길이 제한 10자 초과와 빈칸일 경우 다시 받기
            if (name != "" && name.Length > 10)
            {
                ThrowMessage("10글자 이하로 다시 입력하세요");
                return GetName();
            }

            return name;
        }
    }

    class TownScene : Scene
    {
        public TownScene(Scene parent)
        {
            _name = "마을";
            _comment = "거래, 휴식 등을 할 수 있습니다.";
            _prev = parent;

            _choices = new string[] { "상태보기", "인벤토리", "상점", "던전 입구", "신전" };


            AddScene("Status", new StatusScene(this));
            AddScene("Inventory", new InventoryScene(this));
            AddScene("Shop", new ShopScene(this));
            AddScene("DungunEntrance", new DungunEntranceScene(this));
            AddScene("Temple", new TempleScene(this));

            SetDisplay();
        }

        override public void HandleInput(GameManager game, ConsoleKey key)
        {

            switch (key)
            {
                case ConsoleKey.D0:
                    game.ChangeScene(_prev.Prev);
                    break;

                case ConsoleKey.D1:
                    game.ChangeScene(SceneGroup["Status"]);
                    break;

                case ConsoleKey.D2:
                    game.ChangeScene(SceneGroup["Inventory"]);
                    break;

                case ConsoleKey.D3:
                    game.ChangeScene(SceneGroup["Shop"]);
                    break;

                case ConsoleKey.D4:
                    game.ChangeScene(SceneGroup["DungunEntrance"]);
                    break;

                case ConsoleKey.D5:
                    game.ChangeScene(SceneGroup["Temple"]);
                    break;

                default:
                    ThrowMessage("잘못된 입력입니다.");
                    break;
            }
        }

        void SetDisplay()
        {
            _display = File.ReadAllLines(@"..\..\..\art\Town.txt");
        }

        public override void DrawScene()
        {
            base.DrawScene();
            Screen.Split();
            Screen.DrawTopScreen(Display, 2);
        }
    }

    class StatusScene : Scene
    {
        StatusWidget _statusWidget;
        public StatusScene(Scene parent)
        {
            _name = "능력치";
            _comment = "플레이어의 능력치를 확인합니다.";
            _prev = parent;
            _statusWidget = new StatusWidget(6, 2);
        }

        override public void HandleInput(GameManager game, ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.D0:
                    game.ChangeScene(_prev);
                    break;

                default:
                    ThrowMessage("잘못된 입력입니다.");
                    break;
            }
        }

        public override void Update(GameManager game)
        {
            base.Update(game);
            Player player = game.Player;
            _statusWidget.SetPlayer(player);
        }

        public override void DrawScene()
        {
            base.DrawScene();
            Screen.DrawTopScreen(Display);
            _statusWidget.Draw();
        }
    }

    class InventoryScene : Scene
    {
        GridBox _inventoryWidget;
        public GridBox InventoryWidget { get { return _inventoryWidget; } }
        public InventoryScene(Scene parent)
        {
            _name = "인벤토리";
            _comment = "플레이어의 인벤토리를 확인합니다.";
            _prev = parent;
            _choices = new string[] { "장착관리", "아이템 정렬" };
            _inventoryWidget = new GridBox();
            AddScene("Equip", new EquipScene(this));
        }

        public override void HandleInput(GameManager game, ConsoleKey key)
        {
            base.HandleInput(game, key);
            Player player = game.Player;
            switch (key)
            {
                case ConsoleKey.D0:
                    game.ChangeScene(_prev);
                    break;
                case ConsoleKey.Q:
                    player.ForwardPage();
                    Update(game);
                    DrawScene();
                    break;
                case ConsoleKey.E:
                    player.BackwardPage();
                    Update(game);
                    DrawScene();
                    break;
                case ConsoleKey.D1:
                    game.ChangeScene(SceneGroup["Equip"]);
                    break;

                case ConsoleKey.D2:
                    game.Player.SortInventory();
                    game.RefreshScene();
                    break;

                default:
                    ThrowMessage("잘못된 입력입니다.");
                    break;
            }
        }

        public override void Update(GameManager game)
        {
            base.Update(game);
            Player player = game.Player;
            _inventoryWidget.Clear();
            for (int i = 0 + (player.invenPage * 6); i < ((player.invenPage + 1) * 6) && i < player.Inventory.Count; ++i)
            {
                ItemSlot slot = new ItemSlot();
                slot.SetItem(i, player.Inventory[i]);
                _inventoryWidget.AddItem(slot);
            }
        }

        public override void DrawScene()
        {
            base.DrawScene();
            Screen.DrawTopScreen(Display);

            _inventoryWidget.Draw();
            ShowPagination();
            ShowGold();
        }
    }

    class EquipScene : Scene
    {
        GridBox _inventoryWidget;
        public EquipScene(Scene parent)
        {
            _name = "장착관리";
            _comment = "플레이어의 착용 장비를 관리합니다.";
            _prev = parent;
            _inventoryWidget = ((InventoryScene)parent).InventoryWidget;
        }

        public override void HandleInput(GameManager game, ConsoleKey key)
        {
            if ((key < ConsoleKey.D0 || key >= ConsoleKey.D1 + _choices.Length) && (key != ConsoleKey.Q && key != ConsoleKey.E))
            {
                ThrowMessage("잘못된 입력입니다.");
                return;
            }
            Player player = game.Player;

            switch (key)
            {
                case ConsoleKey.D0:
                    game.ChangeScene(_prev);
                    break;
                case ConsoleKey.Q:
                    player.ForwardPage();
                    Update(game);
                    DrawScene();
                    break;
                case ConsoleKey.E:
                    player.ForwardPage();
                    Update(game);
                    DrawScene();
                    break;

                default:
                    int index = (int)key - 49 + (player.invenPage * 6);
                    if (game.Player.Inventory[index].type == Item.EType.Potion)
                    {
                        ThrowMessage("포션은 장비할 수 없습니다.");
                        return;
                    }
                    else
                    {
                        game.Player.EquipItem(index);
                    }

                    game.RefreshScene();
                    break;
            }
        }

        public override void Update(GameManager game)
        {
            base.Update(game);
            Player player = game.Player;

            SetOption(player);

            SetDisplay(player);
        }

        void SetDisplay(Player player)
        {
            _inventoryWidget.Clear();
            for (int i = 0 + (player.invenPage * 6); i < ((player.invenPage + 1) * 6) && i < player.Inventory.Count; ++i)
            {
                ItemSlot slot = new ItemSlot();
                slot.SetItem(i, player.Inventory[i]);
                _inventoryWidget.AddItem(slot);
            }
        }

        void SetOption(Player player)
        {
            List<string> lines = new List<string>();

            for (int i = 0 + (player.invenPage * 6); i < ((player.invenPage + 1) * 6) && i < player.Inventory.Count; ++i)
            {
                Item item = player.Inventory[i];
                string line = $"{item.Name}";
                if (item.bEquip) line = line.Insert(0, "[E]");
                lines.Add(line);
            }
            _choices = lines.ToArray();
        }

        public override void DrawScene()
        {
            base.DrawScene();
            Screen.DrawTopScreen(Display);
            _inventoryWidget.Draw();
            ShowPagination();
        }
    }

    class ShopScene : Scene
    {
        public Shop shop;
        ShopInformationDeskWidget _widget;

        public ShopScene(Scene parent)
        {
            _name = "상점";
            _comment = "아이템을 구입 또는 판매합니다.";
            _prev = parent;
            _widget = new ShopInformationDeskWidget(35, 3);

            _choices = new string[] { "구입", "판매", "퀘스트" };
            shop = new Shop();


            _display = File.ReadAllLines(@"..\..\..\art\npc.txt");


            AddScene("Buy", new BuyScene(this));
            AddScene("Sell", new SellScene(this));
            AddScene("Quest", new ShopQuestScene(this));
            AddScene("QuestComplete", new ShopQuestCompleteScene(this));
        }

        public override void HandleInput(GameManager game, ConsoleKey key)
        {
            // Set Display
            switch (key)
            {
                case ConsoleKey.D0:
                    game.ChangeScene(_prev);
                    break;

                case ConsoleKey.D1:
                    game.ChangeScene(SceneGroup["Buy"]);
                    break;

                case ConsoleKey.D2:
                    game.ChangeScene(SceneGroup["Sell"]);
                    break;

                case ConsoleKey.D3:
                    if (GameManager.Instance.Player.IsShopQuestComplete())
                        game.ChangeScene(SceneGroup["QuestComplete"]);
                    else
                        game.ChangeScene(SceneGroup["Quest"]);
                    break;

                default:
                    ThrowMessage("잘못된 입력입니다.");
                    break;
            }
        }

        public override void DrawScene()
        {
            base.DrawScene();
            Screen.DrawTopScreen(Display);
            _widget.Draw();
            ShowGold();
        }
    }

    class BuyScene : Scene
    {
        Shop _shop;

        GridBox _ShopItems;

        int _pagination = 0;

        public BuyScene(Scene parent)
        {
            _name = "구입";
            _comment = "아이템을 구입합니다.";
            _prev = parent;
            _shop = ((ShopScene)Prev).shop;
            _ShopItems = new GridBox();
            SetDisplay();
            SetOption();
        }

        public override void HandleInput(GameManager game, ConsoleKey key)
        {
            base.HandleInput(game, key);

            if ((key < ConsoleKey.D0 || key >= ConsoleKey.D1 + _choices.Length) && (key != ConsoleKey.Q && key != ConsoleKey.E))
            {
                ThrowMessage("잘못된 입력입니다.");
                return;
            }
            switch (key)
            {
                case ConsoleKey.D0:
                    game.ChangeScene(_prev);
                    break;

                case ConsoleKey.Q:
                    ForewardItemBundle();
                    SetDisplay();
                    SetOption();
                    DrawScene();
                    break;

                case ConsoleKey.E:
                    BackwardItemBundle();
                    SetDisplay();
                    SetOption();
                    DrawScene();
                    break;

                default:
                    int index = ((int)key - 49) + (_pagination * 6);
                    Item item = _shop.storeItems[index];
                    try
                    {
                        game.Player.Buy(item);
                    }
                    catch (GoldShortageException e)
                    {
                        ThrowMessage("골드가 부족합니다.");
                        return;
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        ThrowMessage("인벤토리가 가득찼습니다.");
                        return;
                    }
                    ThrowMessage($"{item.Name} 을 구입했습니다.");
                    break;
            }
        }

        void SetDisplay()
        {
            _ShopItems.Clear();
            for (int i = 0 + (_pagination * 6); i < _shop.storeItems.Count && i < 6 + (_pagination * 6); ++i)
            {
                ItemSlot slot = new ItemSlot();
                slot.SetItem(i, _shop.storeItems[i]);
                _ShopItems.AddItem(slot);
            }
        }

        void SetOption()
        {
            List<string> lines = new List<string>();
            for (int i = 0 + (_pagination * 6); i < _shop.storeItems.Count && i < 6 + (_pagination * 6); ++i)
            {
                string line = $"{_shop.storeItems[i].Name}";
                lines.Add(line);
            }
            _choices = lines.ToArray();
        }

        public override void DrawScene()
        {
            base.DrawScene();
            Screen.DrawTopScreen(Display);
            _ShopItems.Draw();
            ShowPagination(_pagination + 1, _shop.storeItems.Count / 6 + 1);
            ShowGold();
        }

        // 아이템 번들 전환 시 화면을 다시 그림
        public void ReDrawItem()
        {
            base.DrawScene();
            ShowGold();
        }

        //_pagination 전환 코드
        public void ForewardItemBundle()
        {
            if (_pagination == 0)
            {
                _pagination = (_shop.storeItems.Count / 6);
            }
            else
            {
                _pagination -= 1;
            }
        }
        public void BackwardItemBundle()
        {
            if (_pagination == (_shop.storeItems.Count / 6))
            {
                _pagination = 0;
            }
            else
            {
                _pagination += 1;
            }
        }

    }

    class SellScene : Scene
    {
        GridBox _playerInventory;

        public SellScene(Scene parent)
        {
            _name = "판매";
            _comment = "아이템을 판매합니다.";
            _prev = parent;
            _playerInventory = new GridBox();
        }

        public override void HandleInput(GameManager game, ConsoleKey key)
        {
            base.HandleInput(game, key);

            if ((key < ConsoleKey.D0 || key >= ConsoleKey.D1 + _choices.Length) && (key != ConsoleKey.Q && key != ConsoleKey.E))
            {
                ThrowMessage("잘못된 입력입니다.");
                return;
            }
            Player player = game.Player;

            switch (key)
            {
                case ConsoleKey.D0:
                    game.ChangeScene(_prev);
                    break;

                case ConsoleKey.Q:
                    player.ForwardPage();
                    Update(game);
                    DrawScene();
                    break;
                case ConsoleKey.E:
                    player.ForwardPage();
                    Update(game);
                    DrawScene();
                    break;

                default:
                    int index = (int)key - 49 + (player.invenPage * 6);
                    string ItemName = game.Player.Inventory[index].Name;
                    try
                    {
                        game.Player.Sell(index);
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        return;
                    }
                    catch (EquippedItemException e)
                    {
                        ThrowMessage("장비를 해제 후 판매해주세요.");
                        return;
                    }
                    ThrowMessage($"{ItemName} 을 판매했습니다.");

                    break;
            }
        }

        public override void Update(GameManager game)
        {
            base.Update(game);
            Player player = game.Player;

            SetDisplay(player);
            SetOption(player);
        }

        void SetDisplay(Player player)
        {
            _playerInventory.Clear();
            for (int i = 0 + (player.invenPage * 6); i < ((player.invenPage + 1) * 6) && i < player.Inventory.Count; ++i)
            {
                ItemSlot slot = new ItemSlot();
                slot.SetItem(i, player.Inventory[i]);
                _playerInventory.AddItem(slot);
            }

        }

        void SetOption(Player player)
        {
            List<string> lines = new List<string>();
            for (int i = 0 + (player.invenPage * 6); i < ((player.invenPage + 1) * 6) && i < player.Inventory.Count; ++i)
            {
                Item item = player.Inventory[i];
                string line = $"{item.Name}";
                lines.Add(line);
            }
            _choices = lines.ToArray();
        }

        public override void DrawScene()
        {
            base.DrawScene();
            Screen.DrawTopScreen(Display);
            _playerInventory.Draw();
            ShowPagination();
            ShowGold();
        }
    }

    class ShopQuestScene : Scene
    {
        ShopQuestInfoWidget _widget;
        Quest _quest;
        Player _player;
        List<Quest> _questList;

        public ShopQuestScene(Scene parent)
        {
            _name = "퀘스트";
            _comment = "퀘스트를 수락하거나 거절합니다.";
            _prev = parent;
            _choices = new string[] { "수락", "거절" };
            _display = File.ReadAllLines(@"..\..\..\art\npc.txt");

            _player = GameManager.Instance.Player;
            _questList = GameManager.Instance.QuestList.ShopQuests;
            
            _widget = new ShopQuestInfoWidget(35, 3);
        }

        public override void HandleInput(GameManager game, ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.D0:
                    game.ChangeScene(_prev);
                    break;

                case ConsoleKey.D1:
                    if (_player.PlayerQuest.Name == null)
                    {
                        _player.SetQuest(_quest);
                        _widget.AcceptText(_quest);
                        DrawScene();
                    }
                    else
                        game.ChangeScene(_prev);
                    break;

                case ConsoleKey.D2:
                    if (!_player.PlayerQuest.IsMonsterHuntQuest)
                    {
                        _player.SetQuestNull();

                        _widget.RefuseText();
                        DrawScene();
                        //Thread.Sleep(2000);
                    }
                    else
                        game.ChangeScene(_prev);
                    break;

                default:
                    ThrowMessage("잘못된 입력입니다.");
                    break;
            }
        }

        public override void Update(GameManager game)
        {
            base.Update(game);

            Quest playerQuest = game.Player.PlayerQuest;

            // 플레이어가 Quest를 안가지고 있을 때
            if (playerQuest.Name == null)
            {
                CreateQuest();
                _widget.Init(_quest);
            }
            // 플레이어가 상점 퀘스트를 가지고 있을 때
            else if (!playerQuest.IsMonsterHuntQuest
                && playerQuest.Name != null)
            {
                // 현재 상점 퀘스트는 플레이어가 가지고 있는 퀘스트
                _quest = new Quest(playerQuest.Name, playerQuest.Num,
                    playerQuest.Reward, playerQuest.IsMonsterHuntQuest);
                // 아직 완료되지 않았음을 알려주는 widget
                _widget.IncompletedText(_quest);
            }
            // 플레이어가 신전 퀘스트를 가지고 있을 때
            else if (playerQuest.IsMonsterHuntQuest)
            {
                _widget.AlreadyHaveQuestText();
            }
        }

        public override void DrawScene()
        {
            base.DrawScene();
            Screen.DrawTopScreen(Display);
            _widget.Draw();
        }

        public void CreateQuest()
        {
            Random random = new Random();
            _quest = _questList[random.Next(0, _questList.Count)];
            _player.ShouldCreateShopQuest = false;
        }
    }

    class ShopQuestCompleteScene : Scene
    {
        ShopQuestInfoWidget _widget;
        public ShopQuestCompleteScene(Scene parent)
        {
            _prev = parent;
            _name = "퀘스트";
            _comment = "퀘스트를 완료했습니다!";
            _display = File.ReadAllLines(@"..\..\..\art\npc.txt");

            _widget = new ShopQuestInfoWidget(35, 3);
        }

        public override void HandleInput(GameManager game, ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.D0:
                    game.ChangeScene(_prev);
                    break;

                default:
                    ThrowMessage("잘못된 입력입니다.");
                    break;
            }
        }

        public override void Update(GameManager game)
        {
            base.Update(game);

            _widget.CompletedText(game.Player.PlayerQuest);
            game.Player.GetQuestReward();
            game.Player.SetQuestNull();
        }

        public override void DrawScene()
        {
            base.DrawScene();
            Screen.DrawTopScreen(Display);
            _widget.Draw();
        }
    }

    class TempleScene : Scene
    {
        public TempleScene(Scene parent)
        {
            _name = "신전";
            _comment = "체력을 회복할 수 있습니다.";
            _prev = parent;
            SetDisplay();
            _choices = new string[] { "회복하기 ( 300 G )", "퀘스트" };

            AddScene("TempleQuestComplete", new TempleQuestScene(this));
            AddScene("TempleQuest", new TempleQuestScene(this));
        }

        public override void HandleInput(GameManager game, ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.D0:
                    game.ChangeScene(_prev);
                    break;

                case ConsoleKey.D1:
                    if (game.Player.Rest() == false)
                    {
                        ThrowMessage("골드가 부족합니다.");
                    }
                    else
                    {
                        ThrowMessage("체력을 회복했습니다.");
                    }
                    break;
                case ConsoleKey.D2:
                    if (GameManager.Instance.Player.IsTempleQuestComplete())
                        game.ChangeScene(SceneGroup["TempleQuestComplete"]);
                    else
                        game.ChangeScene(SceneGroup["TempleQuest"]);
                    break;
                default:
                    ThrowMessage("잘못된 입력입니다.");
                    break;
            }
        }

        void SetDisplay()
        {
            _display = File.ReadAllLines(@"..\..\..\art\Church.txt");
        }

        public override void DrawScene()
        {
            base.DrawScene();
            Screen.DrawTopScreen(Display, 5);

            ShowGold();
        }
    }

    class TempleQuestScene : Scene
    {
        TempleQuestInfoWidget _widget;
        Player _player;
        protected Quest _quest;
        List<Quest> _questList;

        public TempleQuestScene(Scene parent)
        {
            _name = "신전 공고문";
            _comment = "퀘스트를 받을 수 있습니다.";
            _prev = parent;

            _choices = new string[] { "수락", "거절" };
            _display = File.ReadAllLines(@"..\..\..\art\parchment.txt");
            _player = GameManager.Instance.Player;

            _widget = new TempleQuestInfoWidget(8, 3);
            _widget.SetSize(25, 14);

            _questList = GameManager.Instance.QuestList.TempleQuests;
        }

        public override void HandleInput(GameManager game, ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.D0:
                    game.ChangeScene(_prev);
                    break;

                case ConsoleKey.D1:
                    if (_player.PlayerQuest.Name == null)
                    {
                        _player.SetQuest(_quest);
                        _widget.AcceptQuestText();
                        DrawScene();
                    }
                    //else if (!_player.PlayerQuest.IsMonsterHuntQuest)
                    //{
                    //    _widget.AlreadyHaveQuestText();
                    //    DrawScene();
                    //    _widget.RefuseQuestText();
                    //}
                    break;

                case ConsoleKey.D2:
                    if (_player.PlayerQuest.IsMonsterHuntQuest)
                    {
                        _player.SetQuestNull();
                        _widget.RefuseQuestText();
                        DrawScene();
                    }
                    
                    break;

                default:
                    ThrowMessage("잘못된 입력입니다.");
                    break;
            }
        }

        public override void Update(GameManager game)
        {
            base.Update(game);

            Quest playerQuest = game.Player.PlayerQuest;

            // 플레이어가 Quest를 안가지고 있을 때
            if (playerQuest.Name == null)
            {
                CreateQuest();
                _widget.Init(_quest);
            }
            // 플레이어가 신전 퀘스트를 가지고 있을 때
            else if (playerQuest.IsMonsterHuntQuest
                && playerQuest.Name != null)
            {
                // 현재 신전 퀘스트는 플레이어가 가지고 있는 퀘스트
                _quest = new Quest(playerQuest.Name, playerQuest.Num,
                    playerQuest.Reward, playerQuest.IsMonsterHuntQuest);

                _widget.Init(_quest);
                // 진행중 text
                _widget.AcceptQuestText();
            }
            // 플레이어가 상점 퀘스트를 가지고 있을 때
            else if (!playerQuest.IsMonsterHuntQuest)
            {
                _widget.AlreadyHaveQuestText();
            }

        }

        public override void DrawScene()
        {
            base.DrawScene();
            Screen.DrawTopScreen(Display);
            _widget.Draw();
            ShowGold();
        }

        public void CreateQuest()
        {
            Random random = new Random();
            _quest = _questList[random.Next(0, _questList.Count)];
            _player.ShouldCreateShopQuest = false;
        }
    }

    class TempleQuestCompleteScene : Scene
    {
        ShopQuestInfoWidget _widget;
        public TempleQuestCompleteScene(Scene parent)
        {
            _prev = parent;
            _name = "퀘스트";
            _comment = "퀘스트를 완료했습니다!";
            _display = File.ReadAllLines(@"..\..\..\art\npc.txt");

            Player player = GameManager.Instance.Player;

            _widget = new ShopQuestInfoWidget(35, 3);

            player.GetQuestReward();
            player.SetQuestNull();
        }

        public override void HandleInput(GameManager game, ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.D0:
                    game.ChangeScene(_prev);
                    break;

                default:
                    ThrowMessage("잘못된 입력입니다.");
                    break;
            }
        }

        public override void Update(GameManager game)
        {
            base.Update(game);

            _widget.CompletedText(game.Player.PlayerQuest);
        }

        public override void DrawScene()
        {
            base.DrawScene();
            Screen.DrawTopScreen(Display);
            _widget.Draw();
        }
    }

    class DungunEntranceScene : Scene
    {
        string[] _recommendDef;

        GridBox _panel;
        public DungunEntranceScene(Scene parent)
        {
            _name = "던전 입구";
            _comment = "입장할 던전을 선택합니다.";
            _prev = parent;
            _panel = new GridBox();
            _panel.SetColomn(1);
            _panel.SetMargine(1, 1);
            _choices = new string[] { "쉬운 던전", "일반 던전", "어려운 던전" };
            _recommendDef = new string[] { "1 ~ 3", "5 ~ 10", "10 ~ 20" };


            for (int i = 0; i < 3; ++i)
            {
                TextBlock textBlock = new TextBlock(2 + 50 * i, 1 + 3 * i, 50, 3);
                string dungeon = Utility.MatchCharacterLength(_choices[i], 20);
                textBlock.SetText($"{i + 1}. {dungeon} | 권장 레벨 {_recommendDef[i]}");
                _panel.AddItem(textBlock);
            }
        }

        public override void HandleInput(GameManager game, ConsoleKey key)
        {
            base.HandleInput(game, key);
            if (game.Player.Hp <= 0 && key != ConsoleKey.D0)
            {
                ThrowMessage("체력이 0 입니다.");
                return;
            }

            switch (key)
            {
                case ConsoleKey.D0:
                    game.ChangeScene(_prev);
                    break;

                case ConsoleKey.D1:
                    game.ChangeScene(new EasyDungeonScene(this));
                    break;

                case ConsoleKey.D2:
                    game.ChangeScene(new NormalDungeonScene(this));
                    break;

                case ConsoleKey.D3:
                    game.ChangeScene(new HardDungeonScene(this));
                    break;

                default:
                    ThrowMessage("잘못된 입력입니다.");
                    break;
            }
        }

        public override void DrawScene()
        {
            base.DrawScene();
            Screen.DrawTopScreen(Display);
            _panel.Draw();
        }
    }

    class BaseDungeonScene : Scene
    {
        protected Dungeon _dungeon;

        public Dungeon Dungeon { get { return _dungeon; } }

        MonsterGridBox _monsters;
        GridBox _selects;
        UnitViewer _playerWidget;
        List<Monster> _monsterList;
        protected int _difficulty;
        BattleWidget battleMsg;

        public BaseDungeonScene()
        {
            _playerWidget = new UnitViewer(3, 19);

            _monsters = new MonsterGridBox();
            _monsters.SetColomn(1);
            _monsters.SetMargine(1, 0);
            
            _selects = new GridBox();
            _selects.SetPosition(0, 24);
            _selects.SetColomn(2);
            _selects.SetMargine(1, 0);


            _choices = new string[] { "공격", "가방" };

            battleMsg = new BattleWidget(2, 25, 50, 5);

            for (int i = 0; i < _choices.Length; ++i)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.SetSize(38, 5);
                textBlock.SetText($"{i + 1}. {_choices[i]}");
                _selects.AddItem(textBlock);
            }

            _monsterList = new List<Monster>();
        }

        protected void SetMonsterCount(Monster[] monsters)
        {
            _monsters.Clear();
            int count = monsters.Length;

            for (int i = 0; i < count; ++i)
            {
                UnitViewer textBlock = new UnitViewer();
                textBlock.SetSize(20, 4);
                textBlock.SetText(monsters[i].Name, monsters[i].Hp, monsters[i].Lv);
                _monsters.AddItem(textBlock);
            }
        }

        public override void HandleInput(GameManager game, ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.D0:
                    if (_dungeon.RunAway())
                    {
                        Screen.DrawBotScreen(new string[] { });
                        battleMsg.SetText("성공적으로 도망쳤습니다.", "", "");
                        battleMsg.Draw();
                        Thread.Sleep(1000);
                        game.ChangeScene(_prev);
                    }
                    else
                    {
                        Screen.DrawBotScreen(new string[] { });
                        battleMsg.SetText("도망가는 것에 실패했습니다.", "", "");
                        battleMsg.Draw();
                        Thread.Sleep(1000);
                        game.ChangeScene(new BattleScene(this, _dungeon));
                    }

                    break;

                case ConsoleKey.D1:
                    game.ChangeScene(new AttackScene(this));
                    break;

                case ConsoleKey.D2:
                    game.ChangeScene(new BagScene(this));
                    break;
                default:
                    ThrowMessage("잘못된 입력입니다.");
                    break;
            }
        }

        public override void Update(GameManager game)
        {
            _dungeon.Enter(game.Player);
            SetMonsterCount(_dungeon.GetMonster());
            _playerWidget.SetText(game.Player.Class, game.Player.Hp, game.Player.Lv);
            _playerWidget.SetSize(30, 4);

            _monsterList = _dungeon.GetMonster().ToList();
        }

        public override void DrawScene()
        {
            Screen.ShowMapName(_name);
            Screen.DrawBotScreen(new string[] { });
            Screen.DrawTopScreen(Display);
            _selects.Draw();

            DrawMonster();

            _monsters.Draw();
            _playerWidget.Draw();
        }

        private void DrawMonster()
        {
            int x, y;
            for (int i = 0; i < _monsterList.Count; ++i)
            {
                if (i == 0) { x = 19; y = 1; }
                else if (i == 1) { x = 55; y = 1; }
                else if (i == 2) { x = 19; y = 11; }
                else { x = 55; y = 11; }

                if (_difficulty == 2) { x = 41; y = 4; }

                for (int j = 0; j < _monsterList[i].Display.Length; ++j)
                {
                    Console.SetCursorPosition(x, y++);
                    Console.Write(_monsterList[i].Display[j]);
                }
            }
        }
    }

    class AttackScene : Scene
    {
        protected Dungeon _dungeon;

        GridBox _skills;
        public AttackScene(Scene parent)
        {
            _prev = parent;

            _skills = new GridBox();
            _skills.SetPosition(0, 24);
            _skills.SetColomn(2);
            _skills.SetMargine(1, 0);
        }

        public override void HandleInput(GameManager game, ConsoleKey key)
        {
            if (key < ConsoleKey.D0 || key >= ConsoleKey.D1 + game.Player.Skills.Count)
            {
                ThrowMessage("잘못된 입력입니다.");
                return;
            }

            switch (key)
            {
                case ConsoleKey.D0:
                    game.ChangeScene(_prev);
                    break;

                default:
                    _dungeon.SelectSkill((int)key - 49);
                    game.ChangeScene(new SelectMonsterScene(this));
                    break;
            }
        }

        public override void Update(GameManager game)
        {
            if (_dungeon == null) _dungeon = ((BaseDungeonScene)_prev).Dungeon;

            _skills.Clear();

            int idx = 1;
            foreach (var skill in game.Player.Skills)
            {
                SkillSlot slot = new SkillSlot();
                slot.SetSkill(idx++, skill);
                _skills.AddItem(slot);
            }
        }

        public override void DrawScene()
        {
            Screen.DrawBotScreen(Option, 3, true);
            _skills.Draw();
        }
    }

    class SelectMonsterScene : Scene
    {
        protected Dungeon _dungeon;
        GridBox _monsters;
        public SelectMonsterScene(Scene parent)
        {
            _prev = parent;

            _monsters = new GridBox();
            _monsters.SetPosition(0, 24);
            _monsters.SetColomn(2);
            _monsters.SetMargine(1, 0);
        }

        public override void HandleInput(GameManager game, ConsoleKey key)
        {
            if (key < ConsoleKey.D0 || key >= ConsoleKey.D1 + _dungeon.GetMonster().Length)
            {
                ThrowMessage("잘못된 입력입니다.");
                return;
            }

            switch (key)
            {
                case ConsoleKey.D0:
                    game.ChangeScene(_prev);
                    break;

                default:
                    if (_dungeon.SelectMonster((int)key - 49))
                    {
                        game.ChangeScene(new BattleScene(this.Prev.Prev, _dungeon));
                    }
                    else
                    {
                        ThrowMessage("해당 몬스터는 죽어있습니다.");
                    }
                    break;
            }
        }

        public override void Update(GameManager game)
        {
            if (_dungeon == null) _dungeon = ((BaseDungeonScene)Prev.Prev).Dungeon;

            _monsters.Clear();
            int idx = 1;
            foreach (var monster in _dungeon.GetMonster())
            {
                UnitViewer slot = new UnitViewer();
                slot.SetSize(38, 5);
                slot.SetText($"{idx++}. {monster.Name}", monster.Hp, monster.Lv);
                _monsters.AddItem(slot);
            }
        }

        public override void DrawScene()
        {
            Screen.DrawBotScreen(Option, 3, true);
            _monsters.Draw();
        }
    }

    class BattleScene : Scene
    {
        Dungeon _dungeon;
        string[] msg;
        Dungeon.EDungeoState state;
        BattleWidget battleMsg;

        public BattleScene(Scene parent, Dungeon dungeon)
        {
            _prev = parent;
            _dungeon = dungeon;
            battleMsg = new BattleWidget(2, 25, 50, 5);
        }

        public override void Update(GameManager game)
        {
            state = _dungeon.Progress(out msg);
            battleMsg.SetText(msg[0], msg[1], msg[2]);
        }

        public override void DrawScene()
        {
            Screen.DrawBotScreen(Option, 3, true);

            battleMsg.Draw();
            Thread.Sleep(2000);

            switch (state)
            {
                case Dungeon.EDungeoState.PlayerTurn:
                    GameManager.Instance.ChangeScene(_prev);
                    break;

                case Dungeon.EDungeoState.MonsterTurn:
                    GameManager.Instance.RefreshScene();
                    break;

                case Dungeon.EDungeoState.PlayerDeath:
                    GameManager.Instance.RefreshScene();
                    break;

                case Dungeon.EDungeoState.MonsterAllDeath:
                    GameManager.Instance.RefreshScene();
                    GameManager.Instance.Player.GetMp(10);
                    break;

                case Dungeon.EDungeoState.Clear:
                    GameManager.Instance.ChangeScene(new RewardScene(_dungeon));
                    break;

                case Dungeon.EDungeoState.GoTown:
                    GameManager.Instance.Player.Revival();
                    GameManager.Instance.ChangeScene(SceneGroup["Town"]);
                    break;
            }
        }
    }

    class RewardScene : Scene
    {
        ResultWidget _resultwidget;
        Reward _reward;
        LevelUpWidget _lvUpWidget;
        bool isLvUp;
        public RewardScene(Dungeon dungeon)
        {
            _name = "탐험 결과";
            _resultwidget = new ResultWidget(2, 0, 35, 23);
            _reward = dungeon.Reward;
            _resultwidget.SetResult(_reward);

            _lvUpWidget = new LevelUpWidget(41, 0, 35, 23);
        }

        public override void HandleInput(GameManager game, ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.D0:
                    game.ChangeScene(SceneGroup["DungunEntrance"]);
                    break;

                default:
                    break;
            }
        }

        public override void Update(GameManager game)
        {
            Player player = game.Player;

            foreach (var item in _reward.Items)
            {
                player.MergeIfConsumable(item);
            }

            player.GetExp(_reward.Exp, out isLvUp);
            player.Gold += _reward.Gold;
        }

        public override void DrawScene()
        {
            base.DrawScene();
            Screen.DrawTopScreen(Display);
            _resultwidget.Draw();
            if (isLvUp)
            {
                Thread.Sleep(1000);
                _lvUpWidget.Draw();
            }
        }
    }

    class BagScene : Scene
    {
        GridBox _itemBox;
        List<int> _itemIndex;
        BattleWidget battleMsg;
        Dungeon _dungeon;

        public BagScene(Scene parent)
        {
            _prev = parent;


            _dungeon = ((BaseDungeonScene)parent).Dungeon;


            _itemBox = new GridBox();
            _itemBox.SetPosition(0, 24);
            _itemBox.SetColomn(2);
            _itemBox.SetMargine(1, 0);

            _itemIndex = new List<int>();

            battleMsg = new BattleWidget(2, 25, 50, 5);
        }

        public override void HandleInput(GameManager game, ConsoleKey key)
        {
            if (key < ConsoleKey.D0 || key >= ConsoleKey.D1 + _itemIndex.Count)
            {
                ThrowMessage("잘못된 입력입니다.");
                return;
            }

            switch (key)
            {
                case ConsoleKey.D0:
                    game.ChangeScene(_prev);
                    break;

                default:
                    int index = _itemIndex[(int)key - 49];
                    Screen.DrawBotScreen(new string[] { });
                    Item item = game.Player.Inventory[index];
                    if (game.Player.UseItem(index))
                    {
                        Screen.DrawBotScreen(Option);
                        string[] msg = MakeSuccessMessage(item);
                        battleMsg.SetText(msg[0], msg[1], msg[2]);
                        battleMsg.Draw();
                        Thread.Sleep(1000);
                        _dungeon.PassPlayerTurn();
                        game.ChangeScene(new BattleScene(Prev, _dungeon));
                    }
                    else
                    {
                        Screen.DrawBotScreen(Option);
                        string[] msg = MakeFailMessage(item);
                        battleMsg.SetText(msg[0], msg[1], msg[2]);
                        battleMsg.Draw();
                        Thread.Sleep(1000);
                        game.RefreshScene();
                    }

                    break;
            }
        }

        public override void Update(GameManager game)
        {
            _itemBox.Clear();
            _itemIndex.Clear();

            int idx = 0;

            for (int i = 0; i < game.Player.Inventory.Count; ++i)
            {
                if (game.Player.Inventory[i].type == Item.EType.Potion)
                {
                    ItemSlot textBlock = new ItemSlot();
                    textBlock.SetSize(38, 5);
                    textBlock.SetItem(idx++, game.Player.Inventory[i]);
                    _itemIndex.Add(i);
                    _itemBox.AddItem(textBlock);
                }
            }


            // 사용할 수 있는 아이템이 없음을 알림 >> ChangeScene 을 호출하지 못해서 갇힘.
        }

        public override void DrawScene()
        {
            //base.DrawScene();
            Screen.DrawBotScreen(Option);
            _itemBox.Draw();
        }

        string[] MakeSuccessMessage(Item item)
        {
            string[] msg = new string[3];
            msg[0] = $"{item.Name} 을(를) 사용합니다.";
            msg[1] = "";

            switch(item._status)
            {
                case Item.EStatus.ATK:
                    break;

                case Item.EStatus.DEF:
                    break;

                case Item.EStatus.HP:
                    msg[2] = $"체력을 {item.Value} 만큼 회복했습니다.";
                    break;
            }
            return msg;
        }

        string[] MakeFailMessage(Item item)
        {
            string[] msg = new string[3];
            msg[0] = $"{item.Name} 을(를) 사용에 실패했습니다.";
            msg[1] = "";

            switch (item._status)
            {
                case Item.EStatus.ATK:
                    break;

                case Item.EStatus.DEF:
                    break;

                case Item.EStatus.HP:
                    msg[2] = "체력이 이미 최대치 입니다.";
                    break;
            }
            return msg;
        }
    }

    class EasyDungeonScene : BaseDungeonScene
    {
        public EasyDungeonScene(Scene parent)
        {
            _difficulty = 0;
            _dungeon = new Dungeon("마을 근처", 0, 2, 2);
            _name = _dungeon.Name;
            _prev = parent;
        }
    }

    class NormalDungeonScene : BaseDungeonScene
    {
        public NormalDungeonScene(Scene parent)
        {
            _difficulty = 1;
            _dungeon = new Dungeon("성벽 외곽", 1, 7, 7);
            _name = _dungeon.Name;
            _prev = parent;
        }
    }

    class HardDungeonScene : BaseDungeonScene
    {
        public HardDungeonScene(Scene parent)
        {
            _difficulty = 2;
            _dungeon = new Dungeon("지하 미궁", 2, 20, 14);
            _name = _dungeon.Name;
            _prev = parent;
        }
    }
}
