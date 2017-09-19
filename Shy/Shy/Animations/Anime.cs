using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Shy.Animations {
    ///<author>Alejandro Cárdenas</author>
    /// <summary>
    /// A class to create animations with ease.
    /// </summary>
    public class Anime {

        public const int TIME = 250;

        public bool isRunning { get; private set; }
        public bool isPaused { get; private set; }

        private int currentAnimation;
        private List<Storyboard> animations;
        private FrameworkElement[] targets;
        private Action animationsCompleted;
        private Action animationsChanged;


        public Anime(FrameworkElement[] elements, AnimeProperties properties) {
            this.targets = elements;
            animations = new List<Storyboard>();

            animations.Add(createStoryboard(targets,properties.time,properties.getProperties()));
        }

        public Anime(FrameworkElement element,AnimeProperties properties) {
            this.targets = new FrameworkElement[] { element };
            animations = new List<Storyboard>();
            animations.Add(createStoryboard(targets,properties.time,properties.getProperties()));
        }

        //public Anime(FrameworkElement[] elements,int time = TIME,params Tuple<String,object>[] properties) {
        //    this.targets = elements;
        //    animations = new List<Storyboard>();
        //    animations.Add(createStoryboard(targets,time,properties));
        //}

        //public Anime(FrameworkElement element,int time = TIME,params Tuple<String,object>[] properties) {
        //    this.targets = new FrameworkElement[] { element };
        //    animations = new List<Storyboard>();
        //    animations.Add(createStoryboard(targets,time,properties));
        //}

        //public Anime then(int time = TIME,params Tuple<String,object>[] properties) {
        //    if (properties.Length > 0)
        //        animations.Add(createStoryboard(targets,time,properties));
        //    return this;
        //}

        public Anime then(AnimeProperties properties) {
            animations.Add(createStoryboard(targets,properties.time,properties.getProperties()));
            return this;
        }

        public Anime completed(Action completed) {
            this.animationsCompleted = completed;
            return this;
        }

        public Anime changed(Action changed) {
            this.animationsChanged = changed;
            return this;
        }

        public void start() {
            if (!isRunning) {
                currentAnimation = 0;
                isRunning = true;
                animations[currentAnimation].Begin(targets[0],true);
            } else if (isPaused) {
                animations[currentAnimation].Resume(targets[0]);
                isPaused = false;
            }
        }

        public void pause() {
            if (isRunning) {
                animations[currentAnimation].Pause(targets[0]);
                isPaused = true;
            }
        }

        private void animationCompleted(object sender,EventArgs e) {
            currentAnimation++;
            if (currentAnimation < animations.Count) {
                animations[currentAnimation].Begin(targets[0],true);
            } else {
                isRunning = false;
                animationsCompleted?.Invoke();
            }
        }


        private Storyboard createStoryboard(FrameworkElement[] elements,int time,IEnumerable<Tuple<String,object>> properties) {
            Storyboard sb = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromMilliseconds(time));

            foreach (var el in elements) {
                foreach (var prop in properties) {
                    Timeline animation = createAnimation(duration, prop.Item2);
                    Storyboard.SetTargetProperty(animation,new PropertyPath(prop.Item1));
                    Storyboard.SetTarget(animation,el);
                    sb.Children.Add(animation);
                }
            }

            sb.Completed += animationCompleted;
            sb.Changed += (s,e) => {
                animationsChanged?.Invoke();
            };
            return sb;
        }

        public Timeline createAnimation(Duration duration,object value) {
            Timeline tl = null;

            if (value is byte) {
                tl = new ByteAnimation((byte)value,duration);
            } else if (value is Int16) {
                tl = new Int16Animation((Int16)value,duration);
            } else if (value is Int32) {
                tl = new Int32Animation((Int32)value,duration);
            } else if (value is Int64) {
                tl = new Int64Animation((Int64)value,duration);
            } else if (value is double) {
                tl = new DoubleAnimation((double)value,duration);
            } else if (value is decimal) {
                tl = new DecimalAnimation((decimal)value,duration);
            } else if (value is Thickness) {
                tl = new ThicknessAnimation((Thickness)value,duration);
            } else if (value is SolidColorBrush) {
                tl = new ColorAnimation((value as SolidColorBrush).Color, duration);
            }

            return tl;
        }

    }


    public class AnimeProperties {
        private Dictionary<String,object> parameters = new Dictionary<string, object>();

        public int time { get; set; } = Anime.TIME;

        public double height {
            get {
                if (parameters.ContainsKey("Height"))
                    return (double)parameters["Height"];
                return 0;
            }
            set {
                parameters["Height"] = value;
            }
        }

        public double width {
            get {
                if (parameters.ContainsKey("Width"))
                    return (double)parameters["Width"];
                return 0;
            }
            set {
                parameters["Width"] = value;
            }
        }
        public double translateX { get; set; }
        public double opacity {
            get {
                if (parameters.ContainsKey("Opacity"))
                    return (double)parameters["Opacity"];
                return 0;
            }
            set {
                parameters["Opacity"] = value;
            }
        }

        public List<Tuple<string,object>> getProperties() {
            List<Tuple<string,object>> par = new List<Tuple<string,object>>();
            foreach (var el in parameters) {
                par.Add(new Tuple<string, object>(el.Key,el.Value));
            }
            return par;
        }

}

}
