﻿using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.Tasks.Geoprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ArcGISRuntimeSDKDotNet_DesktopSamples.Samples
{
    /// <summary>
    /// This sample demonstrates use of the Geoprocessor to call a DriveTimes geoprocessing service. To use the sample, click a point in the map. Drive time polygons of 1, 2, and 3 minutes will be calculated and displayed on the map.
    /// </summary>
    /// <title>Drive Times</title>
	/// <category>Tasks</category>
	/// <subcategory>Geoprocessing</subcategory>
	public partial class DriveTimes : UserControl
    {
        private List<Symbol> _bufferSymbols;
        private Geoprocessor _gpTask;

        /// <summary>Constructs Drive Times sample control</summary>
        public DriveTimes()
        {
            InitializeComponent();

            mapView.Map.InitialExtent = new Envelope(-122.5009, 37.741, -122.3721, 37.8089);

            _bufferSymbols = new List<Symbol>()
            {
                layoutGrid.Resources["FillSymbol1"] as Symbol, 
                layoutGrid.Resources["FillSymbol2"] as Symbol, 
                layoutGrid.Resources["FillSymbol3"] as Symbol
            };

            _gpTask = new Geoprocessor(
                new Uri("http://sampleserver1.arcgisonline.com/ArcGIS/rest/services/Network/ESRI_DriveTime_US/GPServer/CreateDriveTimePolygons"));
        }

        // Use geoprocessor to call drive times gp service and display results
        private async void mapView_MapViewTapped(object sender, MapViewInputEventArgs e)
        {
            try
            {
                progress.Visibility = Visibility.Visible;

                inputLayer.Graphics.Clear();
                resultLayer.Graphics.Clear();

                inputLayer.Graphics.Add(new Graphic(e.Location));

                var parameter = new GPInputParameter();
                parameter.GPParameters.Add(new GPFeatureRecordSetLayer("Input_Location", e.Location));
                parameter.GPParameters.Add(new GPString("Drive_Times", "1 2 3"));

                var result = await _gpTask.ExecuteAsync(parameter);

                var features = result.OutParameters.OfType<GPFeatureRecordSetLayer>().First().FeatureSet.Features;
                resultLayer.Graphics.AddRange(features.Select((fs, idx) => new Graphic(fs.Geometry, _bufferSymbols[idx])));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Sample Error");
            }
            finally
            {
                progress.Visibility = Visibility.Collapsed;
            }
        }
    }
}
