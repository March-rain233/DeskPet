using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 桌宠
{
    /// <summary>
    /// Pet.xaml 的交互逻辑
    /// </summary>
    public partial class Pet : UserControl
    {
        public enum Status
        {
            stand,
            walk,
            idle,
            drag,
            fall,
            climb,
            downToGround
        }
        private Image image;
        private Animator animator;
        private Physics physics;
        private const string imageName = "shime";
        private const string imageExtension = ".png";
        private List<BitmapImage> stand = new List<BitmapImage>();
        private List<BitmapImage> walk = new List<BitmapImage>();
        private List<BitmapImage> drag = new List<BitmapImage>();
        private List<BitmapImage> idle = new List<BitmapImage>();
        private List<BitmapImage> fall = new List<BitmapImage>();
        private List<BitmapImage> climb = new List<BitmapImage>();
        private List<BitmapImage> downToGround = new List<BitmapImage>();
        private Timer timer;
        private Status sta;
        private double vector_L = 1;
        private const double interval = 1;
        int times = 0;
        private bool left;
        public bool Left
        {
            get { return left; }
            private set
            {
                if (value == left) return;
                left = value;
                var translate = GetTT();
                translate.X -= Amend() * 2;
            }
        }

        private double lastTime = 0;
        public Pet()
        {
            InitializeComponent();
            image = pet;
            animator = new Animator(ref image);
            physics = new Physics(ref image);
            timer = new Timer();
            timer.Elapsed += physics.Updata;
            timer.Elapsed += Updata;
            timer.Elapsed += animator.Updata;
            LoadImages(ref stand, 1, 1);
            LoadImages(ref walk, 1, 3);
            LoadImages(ref drag, 5, 7);
            LoadImages(ref idle, 30, 33);
            LoadImages(ref fall, 4, 4);
            LoadImages(ref climb, 12, 14);
            LoadImages(ref downToGround, 18, 19);
            sta = Status.fall;
            AnimationSwitch(Status.fall);
            timer.Interval = interval;
            timer.Enabled = true;
            timer.Start();
        }
        private void Updata(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(delegate
            {
                ChangeDir();
                TransformGroup tg = image.RenderTransform as TransformGroup;
                TranslateTransform translate = tg.Children[1] as TranslateTransform;
                Test.Text = $"{translate.X},{translate.Y}\n{canvas_.ActualWidth}\n{times++}";
                Move();
                Border();
                if(sta == Status.idle||sta == Status.stand || sta == Status.walk)
                {
                    if ((lastTime += 1) >= 200)
                    {
                        RandomAnim();
                        lastTime = 0;
                    }
                }
            }));
        }
        private double Amend()
        {
            if (Left) return image.ActualWidth * 0.5;
            else return -image.ActualWidth * 0.5;
        }
        private void ChangeDir()
        {
            TransformGroup tg = image.RenderTransform as TransformGroup;
            ScaleTransform scale = tg.Children[0] as ScaleTransform;
            if (Left) scale.ScaleX = 1;
            else scale.ScaleX = -1;
        }
        private TranslateTransform GetTT()
        {
            TransformGroup tg = image.RenderTransform as TransformGroup;
            TranslateTransform translate = tg.Children[1] as TranslateTransform;
            return translate;
        }
        private void RandomAnim()
        {
            Random random = new Random();
            var num = random.Next(0, 2);
            switch (num)
            {
                case 0:
                    Left = true;
                    break;
                case 1:
                    Left = false;
                    break;
            }
            num = random.Next(0, 3);
            sta = (Status)num;
            AnimationSwitch(sta);
        }
        private void Move()
        {
            switch (sta)
            {
                case Status.stand:
                    
                    break;
                case Status.walk:
                    Walk();
                    break;
                case Status.drag:
                    
                    break;
                case Status.idle:
                    
                    break;
                case Status.fall:
                    FallMove();
                    break;
                case Status.climb:
                    Climb();
                    break;
                case Status.downToGround:
                    
                    break;
                default:
                    return;
            }
        }
        private void Border()
        {
            var translate = GetTT();
            if (translate.X < 0 - Amend() -0.1) translate.X = - Amend() -0.1;
            if (translate.X > canvas_.ActualWidth - Amend() + 0.1) translate.X = canvas_.ActualWidth - Amend() +0.1;
            if (translate.Y < -0.9) translate.Y = -0.9;
            if (translate.Y > canvas_.ActualHeight - image.ActualHeight * 1.1) translate.Y 
                    = canvas_.ActualHeight - image.ActualHeight * 1.2;
        }
        private bool JudgeClimb()
        {
            var translate = GetTT();
            if (translate.X < - Amend())
            {
                translate.X = - Amend();
                sta = Status.climb;
                Left = true;
                return true;
            }
            if (translate.X > canvas_.ActualWidth - Amend())
            {
                translate.X = canvas_.ActualWidth - Amend();
                sta = Status.climb;
                Left = false;
                return true;
            }
            return false;
        }
        private void FallMove()
        {
            TransformGroup tg = image.RenderTransform as TransformGroup;
            TranslateTransform translate = tg.Children[1] as TranslateTransform;
            bool change = false;
            change = JudgeClimb();
            if (translate.Y < 0) translate.Y = 0;
            if (translate.Y > canvas_.ActualHeight - image.ActualHeight * 1.2)
            {
                translate.Y = canvas_.ActualHeight - image.ActualHeight * 1.2;
                sta = Status.downToGround;
                change = true;
            }
            translate.X += physics.vector.X;
            translate.Y += physics.vector.Y;
            if (change) AnimationSwitch(sta);
        }
        private void Climb()
        {
            TransformGroup tg = image.RenderTransform as TransformGroup;
            TranslateTransform translate = tg.Children[1] as TranslateTransform;
            Random random = new Random();
            int num = random.Next(1, 1000);
            if(num< 800) translate.Y -= vector_L;
            else if(num<995) translate.Y -= vector_L;
            else
            {
                sta = Status.fall;
                AnimationSwitch(Status.fall);
            }
        }
        private void Walk()
        {
            TransformGroup tg = image.RenderTransform as TransformGroup;
            TranslateTransform translate = tg.Children[1] as TranslateTransform;
            if (Left) translate.X -= vector_L;
            else translate.X += vector_L;
            if(JudgeClimb()) AnimationSwitch(sta);
        }
        private void FallDown()
        {
            sta = Status.stand;
            AnimationSwitch(sta);
        }
        /// <summary>
        /// 是并集，范围为[from,to]
        /// </summary>
        private void LoadImages(ref List<BitmapImage> target, uint from, uint to)
        {
            target.Clear();
            for (uint i = from; i <= to; i++)
            {
                target.Add(new BitmapImage(new Uri($@"Resources\{imageName}{i}{imageExtension}", UriKind.Relative)));
            }
        }
        private void AnimationSwitch(Status status)
        {
            switch (status)
            {
                case Status.stand:
                    animator.ResetAnimation(stand, 0.1);
                    break;
                case Status.walk:
                    animator.ResetAnimation(walk, 0.3);
                    break;
                case Status.drag:
                    animator.ResetAnimation(drag, 0.5, true);
                    break;
                case Status.idle:
                    animator.ResetAnimation(idle, 0.5);
                    break;
                case Status.fall:
                    animator.ResetAnimation(fall, 0.1);
                    break;
                case Status.climb:
                    animator.ResetAnimation(climb, 0.3);
                    break;
                case Status.downToGround:
                    animator.ResetAnimation(downToGround, 0.2);
                    animator.animEnd += FallDown;
                    break;
                default:
                    return;
            }
        }

        private void MouseDragElementBehavior_DragBegun(object sender, MouseEventArgs e)
        {
            sta = Status.drag;
            AnimationSwitch(sta);
        }

        private void MouseDragElementBehavior_DragFinished(object sender, MouseEventArgs e)
        {
            sta = Status.fall;
            AnimationSwitch(sta);
        }
        private void Pet_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Pet_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Left = !Left;
        }
    }
    public class Animator
    {
        private Image instance;
        private List<BitmapImage> current;
        private int curFrame = 0;
        private double animPerFrame;
        private double curTime;
        public event Action animEnd;
        private bool ifReverse;
        public Animator(ref Image target)
        {
            instance = target;
            current = null;
        }
        public void Updata(object sender, ElapsedEventArgs e)
        {
            instance.Dispatcher.BeginInvoke(new Action(delegate
            {
                curTime += 10;
                if (curTime >= animPerFrame)
                {
                    PlayAnim();
                    curTime = 0;
                    if(animEnd != null)
                    {
                        animEnd();
                        animEnd -= animEnd;
                    }
                }
            }));
        }
        private void PlayAnim()
        {
            if (current == null) return;
            if (curFrame >= current.Count || curFrame < 0)
            {
                curFrame = 0;
                if (ifReverse)
                {
                    current.Reverse();
                    curFrame = 1;
                }                
            }
            instance.Source = current[curFrame++];
        }
        public void ResetAnimation(List<BitmapImage> source, double duration = 1, bool reverse = false)
        {
            if (source.Count == 0) return;
            animPerFrame = (duration * 1000)/source.Count;
            curTime = 0;
            current = new List<BitmapImage>(source);
            curFrame = 0;
            ifReverse = reverse;
        }
    }
    public class Physics
    {
        private Image instance;
        private const double gravity = 9.8;
        public double gRate = 1;
        private double interval = 100;
        private Point curPoint = new Point(0, 0);
        private Point prePoint = new Point(0, 0);
        public Vector vector { get; private set; }
        public Physics(ref Image image)
        {
            instance = image;
        }
        public void Updata(object sender, ElapsedEventArgs e)
        {
            instance.Dispatcher.BeginInvoke(new Action(delegate
            {
                prePoint = instance.TranslatePoint(new Point(0, 0), instance.Parent as UIElement);
                vector = - new Vector((curPoint - prePoint).X, (curPoint - prePoint).Y) * 0.1 + new Vector(0, gravity * gRate * 0.2);
                curPoint = prePoint;
            }));
        }
    }
}
