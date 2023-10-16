using FosterScript.Core.Agents;
using FosterScript.Core.NeuralNetwork;
using System.Numerics;

namespace FosterScript.Examples.Modules.SmartModules
{
    public class SmartSight : Module
    {
        #region "Inherited Properties"
        public override string Name => "SmartSight";
        public override int[] Version => new int[] { 1, 0, 0 };
        #endregion

        #region "Properties"
        /// <summary>
        /// How inherently good the agent is at seeing.
        /// </summary>
        public double SightQuality { get; set; }

        /// <summary>
        /// How inherently far the agent can see.
        /// </summary>
        public double SightRange { get; set; }
        #endregion

        #region "Private values"
        private Brain brain;
        private SmartEnergy energy;
        private OutputNode sightRangeNeuron;
        private OutputNode sightQualityNeuron;
        #endregion

        public SmartSight() : base()
        {
            Dependencies.Add("SmartEnergy", new int[] { 1, 0, 0 });
            Dependencies.Add("BasicBrain", new int[] { 1, 0, 0 });
        }

        public override void Initialize()
        {
            // Add local values
            brain = (Brain)DependencyReferences["BasicBrain"];
            energy = (SmartEnergy)DependencyReferences["SmartEnergy"];

            // Add input nodes to brain
            InputNode distanceNode = new("Closest target distance", "Gives the hyperbolic tangent of the distance between then actor and the closest target", 0);
            distanceNode.OnRequestOutput += (object sender, InputNeuronEventArgs e) =>
            {
                // TODO: Get closest target
                double distance = 0; // TODO: Put distance here
                e.Output = Math.Tanh((distance - SightRange) / SightRange);
            };
            brain.SupplementingNodes.Add(distanceNode);

            InputNode xNode = new("Distance in X direction to target", "Gives the hyperbolic of the distance in the X direction to the target", 0);
            xNode.OnRequestOutput += (object sender, InputNeuronEventArgs e) =>
            {
                // TODO: Get closest target
                double distance = 0; // TODO: Put distance here
                e.Output = Math.Tanh((distance - SightRange) / SightRange);
            };
            brain.SupplementingNodes.Add(xNode);

            InputNode yNode = new("Distance in Y direction to target", "Gives the hyperbolic of the distance in the Y direction to the target", 0);
            yNode.OnRequestOutput += (object sender, InputNeuronEventArgs e) =>
            {
                // TODO: Get closest target
                double distance = 0; // TODO: Put distance here
                e.Output = Math.Tanh((distance - SightRange) / SightRange);
            };
            brain.SupplementingNodes.Add(yNode);

            // Add output nodes to brain
            sightRangeNeuron = new("Sight Range", "How much to increase/decrease sight range");
            brain.AugmentingNodes.Add(sightRangeNeuron);

            sightQualityNeuron = new("Sight Quality", "How much to increase/decrease quality of sight");
            brain.AugmentingNodes.Add(sightQualityNeuron);
        }

        public override void Think()
        {
            // TODO: Put sight logic here??
        }

        public override void Act()
        {

        }
    }
}
