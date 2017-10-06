using FontAwesome.WPF;
using Shy.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShySamples.Examples {
    /// <summary>
    /// Lógica de interacción para CanonExample.xaml
    /// </summary>
    public partial class CanonExample : UserControl {
        private bool startAnimation;
        private RecBullet currentBullet;
        private double radius;
        private double distance;
        double angle;
        private Random rnd = new Random();
        private Anime animation;
        private Anime animationSize;
        private Brush[] colors = new Brush[] { Brushes.Yellow,Brushes.YellowGreen,Brushes.White,Brushes.LightSteelBlue,Brushes.MediumBlue };


        public CanonExample() {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender,RoutedEventArgs e) {
            grdContainer.MouseDown += GrdContainer_MouseDown;
            grdContainer.MouseUp += GrdContainer_MouseUp;

            currentBullet = new RecBullet();
            //currentBullet.bullet = r;
        }

        private void GrdContainer_MouseUp(object sender,MouseButtonEventArgs e) {
            if (startAnimation) {
                animationSize.pause();
                currentBullet.endMillis = e.Timestamp;
                launchBullet(currentBullet);
                startAnimation = false;
            }
        }

        private void GrdContainer_MouseDown(object sender,MouseButtonEventArgs e) {
            if (!startAnimation) {
                startAnimation = true;
                currentBullet = new RecBullet();
                currentBullet.bullet = createRectangle();
                currentBullet.startMillis = e.Timestamp;
                currentBullet.bullet.Margin = getMargin(e.GetPosition(this));
                grdContainer.Children.Add(currentBullet.bullet);
                animateSize(currentBullet);
            } else {
                currentBullet.force += 1;
            }
            
        }

        private void launchBullet(RecBullet bullet) {
            radius = bullet.force;//this.Width < this.Height ? this.Width : this.Height;
            distance = radius / 3 <= 150 ? 150 : radius / 3;
            angle = rnd.NextDouble() * Math.PI * 2;
            animation = new Anime(new AnimeProperties {
                target = bullet.bullet,
                translateX = Math.Cos(angle) * distance,
                translateY = Math.Sin(angle) * distance,
                height = 0,
                width = 0,
            }).completed((s)=> {
                grdContainer.Children.Remove(s as FrameworkElement);
                Console.WriteLine("animation compleated");
            });

            animation.start();
        }

        private void animateSize(RecBullet bullet) {
            animationSize = new Anime(new AnimeProperties {
                target = bullet.bullet,
                height = 500,
                width = 500,
                time = 2000
            });
            animationSize.start();
        }

        public Thickness getMargin(Point p) {
            Thickness margin = new Thickness();
            margin.Left = p.X;
            margin.Top = p.Y;
            margin.Right = this.ActualWidth - p.X;
            margin.Bottom = this.ActualHeight - p.Y;
            return margin;
        }

        private Image createRectangle() {
            //Rectangle rec = new Rectangle();
            Image img = new Image();
            img.Source = ImageAwesome.CreateImageSource(FontAwesomeIcon.FutbolOutline,colors[rnd.Next(0,colors.Length)]);
            //img.Fill = colors[rnd.Next(0,colors.Length)];
            img.Height = 50;
            img.Width = 50;
            return img;
        }

        struct RecBullet {
            public Image bullet;
            public int startMillis;
            public int endMillis;
            //public int force { get { return endMillis - startMillis; } }
            public int force { get; set; }

        }
    }
}
