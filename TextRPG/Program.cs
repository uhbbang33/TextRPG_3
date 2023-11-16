namespace TextRPG
{
    internal class Program
    {
        public delegate void KeyInputHandler(ConsoleKey key);

        static void Main(string[] args)
        {
            //배경, 글자색 초기화
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            Console.Clear();
            //콘솔창 크기 조정
            Screen.SetSize(80, 40);


            KeyInputHandler keyInputHandle = GameManager.Instance.GetInput;
            GameManager.Instance.RunGame();

            while (GameManager.Instance.isPlay)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;
                    keyInputHandle(key);
                }
                Thread.Sleep(100);
            }
        }
    }
}