using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Zlo4NET.Api.Models.Server;
using Zlo4NET.Api.Models.Shared;

namespace Launcher.Styles.MapAndPlayerRotation
{
    public class MapListViewStyleSelector : StyleSelector
    {
        public MapListViewStyleSelector(IDictionary resources)
        {
            DefaultClassStyle = (Style) resources["DisableSelectionContainerStyle"];
            CurrentClassStyle = (Style) resources["CurrentMapSelectionContainerStyle"];
            NextClassStyle = (Style) resources["NextMapSelectionContainerStyle"];
        }

        public Style DefaultClassStyle { get; }
        public Style CurrentClassStyle { get; }
        public Style NextClassStyle { get; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var map = (ZMap) item;
            switch (map.Role)
            {
                case ZMapRole.Current:
                    return CurrentClassStyle;
                case ZMapRole.Next:
                    return NextClassStyle;
                case ZMapRole.Other:
                default:
                    return DefaultClassStyle;
            }
        }
    }
}