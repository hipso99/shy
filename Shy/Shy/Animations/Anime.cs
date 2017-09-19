using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace Shy.Animations {
    ///<author>Alejandro Cárdenas</author>
    /// <summary>
    /// A class to create animations with ease.
    /// </summary>
    public class Anime {

        private const int TIME = 250;

        public bool isRunning { get; private set; }
        public bool isPaused { get; private set; }

        private int currentAnimation;
        private List<Storyboard> animations;
        private FrameworkElement[] targets;
        private Action animationsCompleted;
        private Action animationsChanged;



        public Anime(FrameworkElement[] elements,int time = TIME,params KeyValuePair<String,object>[] properties) {
            this.targets = elements;
        }

        public Anime(FrameworkElement element,int time = TIME,params Tuple<String,object>[] properties) {
            this.targets = new FrameworkElement[] { element };
            animations = new List<Storyboard>();
            animations.Add(createStoryboard(targets,time,properties));
        }

        public Anime then(int time = TIME,params Tuple<String,object>[] properties) {
            if (properties.Length > 0)
                animations.Add(createStoryboard(targets,time,properties));
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
                targets[0].BeginStoryboard(animations[currentAnimation],HandoffBehavior.SnapshotAndReplace,true);
            } else {
                isRunning = false;
                animationsCompleted?.Invoke();
            }
        }


        private Storyboard createStoryboard(FrameworkElement[] elements,int time,Tuple<String,object>[] properties) {
            Storyboard sb = new Storyboard();
            Duration duration = new Duration(TimeSpan.FromMilliseconds(time));

            foreach (var el in elements) {
                foreach (var prop in properties) {
                    Timeline animation = createAnimation(duration, prop);
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


        public Timeline createAnimation(Duration duration,Tuple<String,object> prop) {
            Timeline tl = null;

            if (prop.Item2 is byte) {
                tl = new ByteAnimation((byte)prop.Item2,duration);
            } else if (prop.Item2 is Int16) {
                tl = new Int16Animation((Int16)prop.Item2,duration);
            } else if (prop.Item2 is Int32) {
                tl = new Int32Animation((Int32)prop.Item2,duration);
            } else if (prop.Item2 is Int64) {
                tl = new Int64Animation((Int64)prop.Item2,duration);
            } else if (prop.Item2 is double) {
                tl = new DoubleAnimation((double)prop.Item2,duration);
            } else if (prop.Item2 is decimal) {
                tl = new DecimalAnimation((decimal)prop.Item2,duration);
            } else if (prop.Item2 is Thickness) {
                tl = new ThicknessAnimation((Thickness)prop.Item2,duration);
            }

            return tl;
        }

    }


}
