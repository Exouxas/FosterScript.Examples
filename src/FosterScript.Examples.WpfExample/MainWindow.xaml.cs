using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
using FosterScript.Examples.Modules;
using FosterScript.Core.Agents;
using FosterScript.Core.Worlds;
using System.Diagnostics;
using FosterScript.Examples.Modules.SmartModules;

namespace FosterScript.Examples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly UIElementCollection _children;
        private readonly Random random = new();
        private readonly Dictionary<Actor, Ellipse> actors = new();

        public MainWindow()
        {
            InitializeComponent();

            _children = ActorCanvas.Children;

            IndefiniteWorld world = new(1000 / 60); // Set refreshrate to 60fps
            int count = 100;


            for (int i = 0; i < count; i++)
            {
                Actor actor = CreateActor(world);

                Ellipse circle = new();
                circle.Width = 10;
                circle.Height = 10;
                circle.Fill = new SolidColorBrush(Color.FromRgb((byte)((255 / count) * (i + 1)), 0, 0));
                circle.Stroke = Brushes.Black;
                circle.StrokeThickness = 1;

                _children.Add(circle);

                actors.Add(actor, circle);

                int r = 100;
                double x = random.NextDouble() * r - r / 2d;
                double y = random.NextDouble() * r - r / 2d;
                actor.Move(new Vector3((float)x, (float)y, 0));
                UpdatePosition(circle, x, y);

            }

            world.StepDone += Tick;
            world.ActorMoved += OnActorMoved;

            world.ActorKilled += (Actor actor, Vector3 vector ) =>
            {
                Debug.WriteLine("Actor died! " + world.Actors.Count + " left");

                Dispatcher.Invoke(() =>
                {
                    _children.Remove(actors[actor]);
                });

                actors.Remove(actor);

                if (world.Actors.Count == 0)
                {
                    world.Stop();
                    Debug.WriteLine("All actors died, stopped world");
                }
            };

            world.Start();
        }

        private void OnActorMoved(Actor actor, Vector3 oldPosition, Vector3 newPosition)
        {
            Ellipse circle = actors[actor];

            UpdatePosition(circle, newPosition.X, newPosition.Y);
        }

        private void Tick()
        {
            // Do something every tick
        }

        private void UpdatePosition(Ellipse circle, double x, double y)
        {
            Dispatcher.Invoke(() =>
            {
                double canvasWidth = Window.Width;
                double canvasHeight = Window.Height;

                double magnitude = 1;
                double leftOffset = canvasWidth / 2 + x * magnitude;
                double topOffset = canvasHeight / 2 + y * magnitude;

                circle.SetValue(Canvas.LeftProperty, leftOffset);
                circle.SetValue(Canvas.TopProperty, topOffset);
            });
        }

        private Actor CreateActor(World world)
        {
            Actor actor = new(world);
            List<Module> modules = new();

            SmartDigestion d = new();
            d.DigestionRate = random.NextDouble() * 2;
            d.StoredMeat = random.NextDouble() * 50;
            d.StoredPlant = random.NextDouble() * 50;
            modules.Add(d);

            SmartEnergy e = new();
            e.EnergyStored = random.NextDouble() * 10;
            modules.Add(e);

            SmartMovement2D mov = new();
            mov.Speed = random.NextDouble() * 1 + 1;
            modules.Add(mov);

            BasicBrain brain = new();
            modules.Add(brain);

            actor.AddModule(modules);
            world.Add(actor);

            return actor;
        }
    }
}
