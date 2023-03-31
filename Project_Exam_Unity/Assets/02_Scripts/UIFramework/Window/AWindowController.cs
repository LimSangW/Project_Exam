namespace Project.UIFramework
{
    public abstract class AWindowController : AWindowController<WindowProperties> { }

    public abstract class AWindowController<TProps> : AUIScreenController<TProps>, IWindowController
        where TProps : IWindowProperties
    {
        public bool HideOnForegroundLost
        {
            get { return Properties.HideOnForegroundLost; }
        }

        public bool IsPopup
        {
            get { return Properties.IsPopup; }
        }

        public WindowPriority WindowPriority
        {
            get { return Properties.WindowQueuePriority; }
        }

        public virtual void UI_Close()
        {
            CloseRequest(this);
        }

        protected sealed override void SetProperties(TProps props)
        {
            if (props != null)
            {
                if (!props.SuppressPrefabProperties)
                {
                    props.HideOnForegroundLost = Properties.HideOnForegroundLost;
                    props.WindowQueuePriority = Properties.WindowQueuePriority;
                    props.IsPopup = Properties.IsPopup;
                }

                Properties = props;
            }
        }

        protected override void HierarchyFixOnShow()
        {
            transform.SetAsLastSibling();
        }
    }
}