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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ShySamples {
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private Anime anime;
        private int maxElements = 250;
        private int duration = 2500;
        private Ellipse[] toAnimate;
        private Brush[] colors = new Brush[] { Brushes.Yellow, Brushes.YellowGreen, Brushes.White, Brushes.LightSteelBlue, Brushes.MediumBlue };
        private double radius;
        private double distance;
        private Random rnd = new Random();
        List<Anime> animations = new List<Anime>();

        public MainWindow() {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender,RoutedEventArgs e) {
            radius = this.Width < this.Height ? this.Width : this.Height;
            distance = radius / 3 <= 150 ? 150 : radius / 3;

            var function = new SineEase() {
                EasingMode = EasingMode.EaseOut,
            };

            grdElements.Visibility = Visibility.Collapsed;
            toAnimate = createEllipses();
            double angle;
            for (int i = 0 ; i < maxElements ; i++) {
                var ellipse = toAnimate[i];
                angle = rnd.NextDouble() * Math.PI * 2;
                Console.WriteLine(angle);
                grdElements.Children.Add(ellipse);
                var animation = new Anime(new AnimeProperties {
                    target = ellipse,
                    translateX = Math.Cos(angle) * distance,
                    translateY = Math.Sin(angle) * distance,
                    height = 0,
                    width = 0,
                    time = duration,
                    easing = function,
                    repeat = RepeatBehavior.Forever,
                    delay = (duration / maxElements) * i * 10,
                });
                animations.Add(animation);
                //animation.start();
            }
            grdElements.Visibility = Visibility.Visible;
        }

        private Ellipse createEllipse() {
            Ellipse e = new Ellipse();
            e.Height = 50;
            e.Width = 50;
            e.Fill = colors[rnd.Next(0,colors.Length)];
            return e;
        }

        private Ellipse[] createEllipses() {
            Ellipse[] ellipses = new Ellipse[maxElements];
            for (int i = 0 ; i < maxElements ; i++) {
                ellipses[i] = createEllipse();
            }
            return ellipses;
        }



            //anime = new Anime(new AnimeProperties {
            //    target = rectangle,
            //    translateX = new double[] { -250 , 250 },
            //    easing = function,
            //    repeat = RepeatBehavior.Forever,
            //});

                //anime = new Anime(new AnimeProperties {
                //    target = rectangle,
                //    //width = rectangle.ActualWidth + 50,
                //    width = new DoubleAnimeProperty {
                //        to = rectangle.ActualWidth + 50,
                //        easing = function,
                //    },
                //    height = rectangle.ActualHeight - 5,
                //    translateX = new double[] {0, 250},
                //    easing = function,
                //}).then(new AnimeProperties {
                //    translateX = new double[] { 205, -250 },
                //    width = rectangle.ActualWidth + 20,
                //    height = rectangle.ActualHeight - 5,
                //    //easing = function,
                //}).then(new AnimeProperties {
                //    width = rectangle.ActualWidth + 20,
                //    translateX = 100,
                //    //easing = function,
                //    //height = rectangle.ActualHeight + 5,
                //}).then(new AnimeProperties {
                //    width = rectangle.Width - 10,
                //    height = rectangle.Height + 10,
                //    translateX = 0,
                //    time = 125,
                //    //easing = function,
                //}).then(new AnimeProperties {
                //    width = rectangle.Width + 10,
                //    height = rectangle.Height,
                //    //easing = function,
                //}).then(new AnimeProperties {
                //    width = rectangle.Width,
                //    easing = function,
                //    //height = rectangle.Height,
                //});
        //}


        private void start_Click(object sender,RoutedEventArgs e) {
            foreach (var anime in animations) {
                if (!anime.isRunning) {
                    anime.start();
                } else if (!anime.isPaused) {
                    anime.pause();
                } else if (anime.isPaused) {
                    anime.start();
                }
            }
        }
    }
}
