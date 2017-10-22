using VRTK;

namespace ASL.Manipulation.Controllers.Vive
{
    // NOTE: This script must be integrated with Martin's work on auto
    // ownership transferral and moved from the controller folder to the
    // networking folder.

    /// <summary>
    /// A script that should be added to any synchronized object that a Vive player
    /// can interact with - thus when the Vive player touches the object, the player can
    /// request ownership of it 
    /// </summary>
    public class ObjectOwnership : VRTK_InteractableObject
    {
        /// <summary>
        /// On start, subscribe OnInteractableObjectTouchedHandler to the on touched event 
        /// </summary>
        protected void Start()
        {
            // Subscribe to on touched event 
            InteractableObjectTouched += OnInteractableObjectTouchedHandler;
        }

        /// <summary>
        /// Called whenever the object is touched by a Vive controller
        /// The Vive player requests ownership of this object
        /// </summary>
        /// <param name="obj">Reference to the Vive controller</param>
        /// <param name="e">Interactable object event arguments</param>
        public void OnInteractableObjectTouchedHandler(object obj, InteractableObjectEventArgs e)
        {
            gameObject.GetPhotonView().RequestOwnership();
        }
    }
}
