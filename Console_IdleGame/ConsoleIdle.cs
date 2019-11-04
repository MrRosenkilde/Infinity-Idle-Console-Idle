using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using static InfiniteNumbers.ConsoleIdle;

namespace InfiniteNumbers
{
    public class ConsoleIdle
    {
        public static object _lock = new object();
        public static void Main(string[] args)
        {
            var render = new ConsoleRender();
            var score = new Score();
            var click_data = new ClickData();

            var items = new List<Item> {
                new Item (10, 0.1m, "First Item" ),
                new Item (50, 1.1m, "Second Item" )
            };
            var focus = new Focus(render);
            State state = new State(render)
            {
                ClickData = click_data,
                Score = score,
                Items = items,
                Focus = focus
            };
            click_data.OnClick = (state) =>
            {
                state.Score.Value += click_data.ClickValue;
                click_data.TotalClicks++;
                render.Update();
            };
            render.Add(score);
            render.Add(state);
            render.Add(click_data);
            items.ForEach(item =>
            {
                render.Add(item);
                focus.Add(item);
            });
            render.Add(new Hints());
            new TimerBasedAction(1000, () =>
            {
                score.Value += state.TotalIncome;
                render.Update();
            }).Start();
            new Thread(
                new ThreadStart(() =>
                {
                Take_User_Input:
                    var input = Console.ReadKey(true).Key;
                    if (UserInputs.InputMap.TryGetValue(input, out Action<State> action))
                    {
                        action.Invoke(state);
                        render.Update();
                    }
                    goto Take_User_Input;
                })
            ).Start();

            var _quitEvent = new ManualResetEvent(false);

            Console.CancelKeyPress += (sender, eArgs) => {
                _quitEvent.Set();
                eArgs.Cancel = true;
            };
            // kick off asynchronous stuff 
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            render.Update();
            _quitEvent.WaitOne();
        }
        public class ConsoleRender
        {
            int counter;
            public IList<Writable> toWrite;
            private bool Updating = false;
            public ConsoleRender()
            {
                counter = 0;
                toWrite = new List<Writable>();
            }
            public void Update()
            {
                if (!Updating) {
                    Updating = true;
                    Console.Clear();
                    foreach (var r in toWrite)
                    {
                        r.Write();
                    }
                    Updating = false;
                }
            }
            public IList<Writable> Add(params Writable[] renderable)
            {
                foreach (var r in renderable)
                    toWrite.Add(r);
                return toWrite;
            }

        }
    }
    public class TimerBasedAction
    {
        public Action Action;
        public int Interval { get; set; }
        public TimerBasedAction(int interval, Action action)
        {
            Interval = interval;
            Action = action;
        }
        public void Start()
        {
            System.Timers.Timer t = new System.Timers.Timer(Interval);
            t.AutoReset = true;
            t.Elapsed += new ElapsedEventHandler((sender, e) => Action.Invoke());
            t.Start();
        }
    }
    public interface Writable
    {
        void Write();
    }
    public class Focus
    {
        public IList<IFocusable> Items { get; }
        private int Index = 0;
        private readonly ConsoleRender Render;
        public Focus(ConsoleRender render)
        {
            Items = new List<IFocusable>();
            this.Render = render;
        }
        public void Add(IFocusable f)
        {
            Items.Add(f);
            if (Items.Count == 1) f.Focus(); //if first added item, set focus
        }
        public void Remove(IFocusable f) => Items.Remove(f);
        public void FocusNext()
        {
            Focused.RemoveFocus();
            Index = Index == Items.Count - 1 ? 0 : ++Index;
            Focused.Focus();
            Render.Update();
        }
        public void FocusPrevious()
        {
            Focused.RemoveFocus();
            Index = Index == 0 ? Items.Count - 1 : --Index;
            Focused.Focus();
            Render.Update();
        }
        public IFocusable Focused { get => Items[Index]; private set => Focused = value; }


    }
    public interface IFocusable
    {
        bool HasFocus();
        void Focus();
        void RemoveFocus();
    }

    public class Score : Writable
    {
        public InfinityNumber Value { get; set; } = 0m;
        public void Write() => Console.Write("Score: " + Value.ToString() + "\n");
    }
    public class Item : Writable, IFocusable
    {
        public decimal BasePrice { get; set; }
        public decimal BaseIncome { get; set; }
        public double PriceIncrease { get; set; } = 1.15;
        public InfinityNumber Price { get; set; }
        public InfinityNumber Income { get; set; }
        public InfinityNumber Amount { get; set; } = 0m;
        public decimal IncomeMultiplier { get; set; } = 1;
        public String Name { get; set; }
        private bool Focused { get; set; }
        public Item(decimal basePrice, decimal baseIncome, String name)
        {
            BasePrice = basePrice;
            BaseIncome = baseIncome;
            Price = BasePrice;
            Income = 0m;
            Name = name;
        }

        public void Write()
        {
            Console.ForegroundColor = Focused ? ConsoleColor.Green : ConsoleColor.White;
            Console.Write(
                    " *********************** \n" +
                   "* " + Name + "\t\t* \n" +
                   "*\t" + "Price\t " + Price.ToString() + "\t*\n" +
                   "*\tIncome\t " + Income.ToString() + "\t*\n" +
                    "*\tAmount\t " + Amount + "\t*\n" +
                   " *********************** \n\n");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public bool HasFocus() => Focused;

        public void Focus() => Focused = true;

        public void RemoveFocus() => Focused = false;
    }
    public class ClickData : Writable
    {
        public int TotalClicks { get; set; } = 0;
        public InfinityNumber ClickValue { get; set; } = 1m;
        public Action<State> OnClick { get; set; }

        public void Write()
        {
            Console.WriteLine("\nClick Data\t\n" +
                "Total Clicks = \t" + TotalClicks + "\n" +
                "Click Value = \t" + ClickValue + "\n" +
                "Press C to click\n");
        }
    }
    public class Hints : Writable
    {
        public void Write()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n" +
                "Press C to click \n" +
                "Use arrow keys (left, right) to focus items\n" +
                "Press B to buy an item an increase income\n");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
    public class State : Writable
    {
        public Score Score { get; set; }
        public ClickData ClickData { get; set; }
        public IList<Item> Items { get; set; }
        private ConsoleRender Render;
        public State(ConsoleRender render)
        {
            Render = render;
        }
        public void BuyFocused()
        {
            var focusedElement = Focus.Focused;
            if (focusedElement is Item)
            {
                var item = (Item)focusedElement;
                if (item.Price <= Score.Value)
                {
                    Score.Value -= item.Price;
                    item.Income += item.BaseIncome * item.IncomeMultiplier;
                    item.Amount++;
                    item.Price = (decimal)Math.Pow(item.PriceIncrease, (int)item.Amount) * item.BasePrice;
                    TotalIncome += item.BaseIncome * item.IncomeMultiplier;
                    Render.Update();
                }
            }
            
        }

        public void Write()
        {
            Console.Write("\nGame Data: \n" +
                "Income /s = \t" + TotalIncome + "\n\n");
        }

        public Focus Focus { get; set; }
        public decimal TotalIncome { get; set; } = 1;
    }
    public sealed class UserInputs
    {
        public static readonly IDictionary<ConsoleKey, Action<State>> InputMap = new Dictionary<ConsoleKey, Action<State>>
        {
            { ConsoleKey.B , (state) => state.BuyFocused() },
            { ConsoleKey.LeftArrow, (state) => state.Focus.FocusPrevious() },
            { ConsoleKey.RightArrow, (state) => state.Focus.FocusNext() },
            { ConsoleKey.C, (state) => state.ClickData.OnClick.Invoke(state) }
        };
    }
}
