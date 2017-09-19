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

namespace ShySamples {
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private Anime anime;
        public MainWindow() {
            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender,RoutedEventArgs e) {
            anime = new Anime(
                element: rectangle,
                properties: new AnimeProperties {
                    height = rectangle.ActualHeight + 50,
                    //opacity = 0,
                    time = 300,
            }).then(new AnimeProperties {
                width = rectangle.ActualWidth + 500,
                //opacity = 1,
                time = 500,
            }).then(new AnimeProperties {
                width = rectangle.Width,
                height = rectangle.Height,
            });
            /*anime = new Anime(
                element: rectangle,
                time:500,
                properties: new [] {
//                    new Tuple<String,object>("Opacity",0.0),
                    new Tuple<String,object>("Height",rectangle.ActualHeight + 50)
                }
            ).then(
                time: 500,
                properties: new[] {
  //                  new Tuple<String,object>("Opacity",1.0),
                    new Tuple<String,object>("Width",rectangle.ActualWidth + 500),
                    new Tuple<String,object>("Fill.Color",Brushes.Beige),
                }
            ).then(
                properties: new[] {
                    new Tuple<String,object>("Width",rectangle.Width),
                    new Tuple<String,object>("Height",rectangle.Height),
                    new Tuple<String,object>("Fill.Color",Brushes.Azure),

                }
            );*/
        }

        private void start_Click(object sender,RoutedEventArgs e) {
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
