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
            anime = new Anime(new AnimeProperties {
                target = rectangle,
                //time = 500,
                width = rectangle.ActualWidth + 50,
                height = rectangle.ActualHeight - 5,
                translateX = 250,
            }).then(new AnimeProperties {
                //time = 500,
                translateX = -250,
                width = rectangle.ActualWidth + 20,
                height = rectangle.ActualHeight - 5,
        }).then(new AnimeProperties {
                //time = 500,
                width = rectangle.ActualWidth + 20,
                translateX = 100,
                height = rectangle.ActualHeight + 5,
        }).then(new AnimeProperties {
                //time = 500,
                width = rectangle.Width,
                height = rectangle.Height,
                translateX = 50,
            }).then(new AnimeProperties {
                //time = 500,
                translateX = 0,
            });
            //anime = new Anime(
            //    element: rectangle,
            //    properties: new AnimeProperties {
            //        height = rectangle.ActualHeight + 50,
            //        time = 300,

            //}).then(new AnimeProperties {
            //    width = rectangle.ActualWidth + 500,
            //    time = 500,
            //}).then(new AnimeProperties {
            //    width = rectangle.Width,
            //    height = rectangle.Height,
            //});
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
