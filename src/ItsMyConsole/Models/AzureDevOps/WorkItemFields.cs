namespace ItsMyConsole
{
    /// <summary>
    /// Champs à modifier sur un WorkItem
    /// </summary>
    public class WorkItemFields
    {
        /// <summary>
        /// Zone
        /// </summary>
        public string AreaPath { get; set; }

        /// <summary>
        /// Projet
        /// </summary>
        public string TeamProject { get; set; }

        /// <summary>
        /// Itération
        /// </summary>
        public string IterationPath { get; set; }

        /// <summary>
        /// Titre
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// État
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        public string WorkItemType { get; set; }

        /// <summary>
        /// Assigner à une personne
        /// </summary>
        public string AssignedTo { get; set; }

        /// <summary>
        /// Activité
        /// </summary>
        public string Activity { get; set; }
    }
}
