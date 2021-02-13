using FamilyTreeLibrary.FamilyData;
using FamilyTreeLibrary.FamilyTreeStore;
using SharpKml.Base;
using SharpKml.Dom;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FamilyTreeWebTools.Services
{
  public class MapExportClass
  {
    static TraceSource trace = new TraceSource("MapExportClass", SourceLevels.Warning);

    private static Style CreateStyle(IndividualEventClass.EventType evType)
    {
      string iconUrl = null;

      Style style = new Style();
      style.Icon = new IconStyle();
      int color = 0;

      switch (evType)
      {
        case IndividualEventClass.EventType.Birth:
        case IndividualEventClass.EventType.Baptism:
          style.Id = "Birth";
          iconUrl = "http://maps.google.com/mapfiles/kml/pal4/icon47.png";
          color = 0x7F00FF00;
          break;

        case IndividualEventClass.EventType.Death:
        case IndividualEventClass.EventType.Burial:
          style.Id = "Death";
          iconUrl = "http://maps.google.com/mapfiles/kml/pal4/icon63.png";
          color = 0x7FFF0000;
          break;

        default:
          style.Id = "Other";
          color = 0x7F0000FF;
          iconUrl = "http://maps.google.com/mapfiles/kml/pal3/icon21.png";
          break;
      }

      //style.Icon.Color = new Color32(255, 0, 255, 0);
      style.Icon.Color = new Color32(color);
      style.Icon.ColorMode = ColorMode.Random;
      style.Icon.Icon = new IconStyle.IconLink(new Uri(iconUrl));
      style.Icon.Scale = 1.0;
      return style;
    }

    public static string CreateMapFile(IFamilyTreeStoreBaseClass familyTree)
    {
      IEnumerator<IndividualClass> personIterator = familyTree.SearchPerson();

      //Kml kml = new Kml();
      var document = new Document();

      while (personIterator.MoveNext())
      {
        IndividualClass person = personIterator.Current;

        string personName = person.GetPersonalName().ToString();

        foreach (IndividualEventClass ev in person.GetEventList())
        {
          string evDescription = personName + " " + ev.GetEventType().ToString() + " " + ev.GetDate().ToString();

          AddressClass address = ev.GetAddress();
          Placemark placemark = null;
          if (address != null)
          {
            string addressString = null;
            addressString = address.ToString();

            placemark = new Placemark();
            placemark.Name = evDescription;
            placemark.Address = addressString;
            //placemark.Description.AddChild(addressString);

            trace.TraceData(TraceEventType.Information, 0, "Address:" + addressString);

            //placemark.AddStyle(CreateStyle(evDescription, ev.GetEventType()));
            // Package it all together...
            //document.AddFeature(placemark);
          }

          PlaceStructureClass place = ev.GetPlace();
          if (place != null)
          {
            MapPosition position = place.GetMapPosition();
            if (position != null)
            {
              if (!Double.IsNaN(position.latitude) && !Double.IsNaN(position.longitude))
              {
                trace.TraceData(TraceEventType.Information, 0, "Creating a point at " + position.latitude + " " + position.longitude);

                if (placemark == null)
                {
                  placemark = new Placemark();
                  placemark.Name = evDescription;
                  //placemark.Time = new Timestamp(ev.GetDate().ToDateTime()); //ev.GetDate().ToDateTime().ToString()));
                }
                // This will be used for the placemark
                Point point = new Point();
                point.Coordinate = new Vector(position.latitude, position.longitude);

                //trace.TraceData(TraceEventType.Information, 0, "Latitude:" + point.Coordinate.Latitude + " Longitude:" +  point.Coordinate.Longitude);

                placemark.Geometry = point;

                // Add the marker 
              }
            }
          }
          if (placemark != null)
          {
            placemark.AddStyle(CreateStyle(ev.GetEventType()));
            document.AddFeature(placemark);
          }
        }
      }
      Serializer serializer = new Serializer();
      serializer.Serialize(document);
      //Console.WriteLine(serializer.Xml);

      return serializer.Xml;
    }
  }
}
