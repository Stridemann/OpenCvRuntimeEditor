namespace OpenCvRuntimeEditor.ViewModels.Nodes
{
    using System;
    using System.Windows;
    using JetBrains.Annotations;
    using Settings;

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class BaseGridObject : BaseViewModel
    {
        private Point _pos;

        public Point Pos
        {
            get => _pos;
            set => SetGridPos(value);
        }

        private void SetGridPos(Point pos)
        {
            var gridSize = 200d / GeneralSettings.Instance.GridMoveStepSize;
            pos.X = (int) Math.Round((int) Math.Round(pos.X / gridSize) * gridSize);
            pos.Y = (int) Math.Round((int) Math.Round(pos.Y / gridSize) * gridSize);
            _pos = pos;
            RaisePropertyChanged(nameof(Pos));
        }
    }
}
