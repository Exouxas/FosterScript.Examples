using FosterScript.Core.Agents;
using FosterScript.Core.NeuralNetwork;
using System.Numerics;

namespace FosterScript.Examples.Modules.SmartModules
{
    public class SmartHealth : Module
    {
        #region "Inherited Properties"
        public override string Name => "SmartHealth";
        public override int[] Version => new int[] { 1, 0, 0 };
        #endregion

        #region "Properties"
        /// <summary>
        /// The current health of the agent.
        /// </summary>
        public double CurrentHealth { get; set; }

        /// <summary>
        /// The maximum health of the agent.
        /// </summary>
        public double MaxHealth { get; set; }

        /// <summary>
        /// Health type.
        /// </summary>
        public Substance HealthType { get; set; }

        public enum Substance
        {
            Plant,
            Meat,
        }
        #endregion

        #region "Private values"
        private Brain brain;
        private SmartEnergy energy;
        private OutputNode regenRateNeuron;
        private OutputNode maxHealthIncreaseNeuron;
        #endregion

        public SmartHealth() : base()
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
            InputNode healthLevelNeuron = new("Health", "Gives the percentage of health", 0);
            healthLevelNeuron.OnRequestOutput += (object sender, InputNeuronEventArgs e) =>
            {
                e.Output = CurrentHealth / MaxHealth;
            };
            brain.SupplementingNodes.Add(healthLevelNeuron);

            InputNode healthToEnergyRatioNeuron = new("Health To Energy Ratio", "Gives the hyperbolic of the health to energy ratio", 0);
            healthToEnergyRatioNeuron.OnRequestOutput += (object sender, InputNeuronEventArgs e) =>
            {
                e.Output = Math.Tanh(energy.EnergyStored / MaxHealth);
            };
            brain.SupplementingNodes.Add(healthToEnergyRatioNeuron);

            // Add output nodes to brain
            regenRateNeuron = new("Regen Rate", "How much to increase health");
            brain.AugmentingNodes.Add(regenRateNeuron);

            maxHealthIncreaseNeuron = new("Max-Health Increase", "How much to increase max health");
            brain.AugmentingNodes.Add(maxHealthIncreaseNeuron);
        }

        public override void Think()
        {

        }

        public override void Act()
        {
            // If out of health, kill the actor
            if (CurrentHealth <= 0)
            {
                Body?.Kill();
            }

            // Increase max health
            double maxHealthIncrease = Math.Max(maxHealthIncreaseNeuron.Result, 0) * MaxHealth;
            MaxHealth += maxHealthIncrease;
            energy.EnergyStored -= maxHealthIncrease * maxHealthIncrease;

            // Regenerate health
            double regenRate = Math.Max(regenRateNeuron.Result, 0) * (MaxHealth - CurrentHealth);
            CurrentHealth += regenRate;
            energy.EnergyStored -= regenRate * regenRate;
        }
    }
}
