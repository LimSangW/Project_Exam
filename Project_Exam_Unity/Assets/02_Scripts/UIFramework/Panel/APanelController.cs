namespace Project.UIFramework
{
    public abstract class APanelController : APanelController<PanelProperties> { }

    public abstract class APanelController<T> : AUIScreenController<T>, IPanelController where T : IPanelProperties
    {
        public PanelPriority Priority
        {
            get
            {
                if (Properties != null)
                {
                    return Properties.Priority;
                }
                else
                {
                    return PanelPriority.None;
                }
            }
        }

        protected sealed override void SetProperties(T props)
        {
            base.SetProperties(props);
        }
    }
}
