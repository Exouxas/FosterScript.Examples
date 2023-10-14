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
        private readonly IndefiniteWorld world = new(1000 / 60); // Set refreshrate to 60fps

        public MainWindow()
        {
            InitializeComponent();

            _children = ActorCanvas.Children;

            world.StepDone += Tick;
            world.ActorMoved += OnActorMoved;
            world.ActorKilled += OnActorKilled;


            int count = 100;
            for (int i = 0; i < count; i++)
            {
                int r = 100;
                double x = random.NextDouble() * r - r / 2d;
                double y = random.NextDouble() * r - r / 2d;

                Actor actor = CreateActor(world, x, y);
            }


            world.Start();
        }

        private void OnActorMoved(Actor actor, Vector3 oldPosition, Vector3 newPosition)
        {
            Ellipse circle = actors[actor];

            UpdatePosition(circle, newPosition.X, newPosition.Y);
        }

        private void OnActorKilled(Actor actor, Vector3 vector)
        {
            Debug.WriteLine("Actor died! " + world.Actors.Count + " left");

            // Remove circle from canvas
            Dispatcher.Invoke(() =>
            {
                if (_children.Contains(actors[actor]))
                {
                    _children.Remove(actors[actor]);
                }
            });

            // Remove actor and circle from dictionary
            actors.Remove(actor);

            // Create death marker
            Dispatcher.Invoke(() =>
            {
                Vector3 center = new((float)Window.Width / 2, (float)Window.Height / 2, 0);
                foreach (Shape shape in CreateDeathMarker(vector + center))
                {
                    _children.Add(shape);
                }
            });

            if (world.Actors.Count == 0)
            {
                world.Stop();
                Debug.WriteLine("All actors died, stopped world");
            }
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
                double leftOffset = canvasWidth / 2 + x * magnitude - circle.Width / 2;
                double topOffset = canvasHeight / 2 + y * magnitude - circle.Height / 2;

                circle.SetValue(Canvas.LeftProperty, leftOffset);
                circle.SetValue(Canvas.TopProperty, topOffset);
            });
        }

        private Actor CreateActor(World world, double x, double y)
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

            Ellipse circle = new();
            circle.Width = 10;
            circle.Height = 10;
            byte red = (byte)(255 * random.NextDouble());
            byte green = (byte)(255 * random.NextDouble());
            byte blue = (byte)(255 * random.NextDouble());
            circle.Fill = new SolidColorBrush(Color.FromRgb(red, green, blue));
            circle.Stroke = Brushes.Black;
            circle.StrokeThickness = 1;

            _children.Add(circle);
            actors.Add(actor, circle);
            actor.Move(new Vector3((float)x, (float)y, 0));

            return actor;
        }

        private List<Shape> CreateDeathMarker(Vector3 position)
        {
            double size = 5;
            List<Shape> shapes = new();

            Line l1 = new();
            l1.X1 = position.X - size;
            l1.X2 = position.X + size;
            l1.Y1 = position.Y - size;
            l1.Y2 = position.Y + size;
            l1.Stroke = Brushes.Black;
            l1.StrokeThickness = 1;
            shapes.Add(l1);

            Line l2 = new();
            l2.X1 = position.X + size;
            l2.X2 = position.X - size;
            l2.Y1 = position.Y - size;
            l2.Y2 = position.Y + size;
            l2.Stroke = Brushes.Black;
            l2.StrokeThickness = 1;
            shapes.Add(l2);

            return shapes;
        }
    }
}
