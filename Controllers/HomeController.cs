using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;
using MongoDB.Driver.GeoJsonObjectModel;
using NearAirports.Models;
using NearAirports.Models.ViewModels;
using NearAirports.Repository;
using NearAirports.Settings;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace NearAirports.Controllers
{
    public class HomeController : Controller
    {
        public readonly AirportRepository repository;

        public readonly string apiKey;

        public HomeController(AirportRepository repository, IOptions<MapsApiSettings> apiSettings)
        {
            this.apiKey = apiSettings.Value.ApiKey;
            this.repository = repository;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var coordenadas = new Coordenada("São Paulo", "-23.64340873969638", "-46.730219057147224");
            return View(coordenadas);
        }

        public async Task<JsonResult> Localizar(HomeViewModel model)
        {
            Coordenada coordLocal = ObterCoordenadasDaLocalizacao(model.Endereco);
            var aeroportosProximos = new List<Coordenada>();
            aeroportosProximos.Add(coordLocal);

            Console.WriteLine(model);

            double lat = Convert.ToDouble(coordLocal.Latitude.Replace(".", ","));
            double lon = Convert.ToDouble(coordLocal.Longitude.Replace(".", ","));
            var ponto = new GeoJson2DGeographicCoordinates(lon, lat);
            var localizacao = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(ponto);
            int distancia = model.Distancia * 1000;

            string? tipoAero =
                (model.Tipo == TipoAeroporto.Internacionais) ? "International" :
                    (model.Tipo == TipoAeroporto.Municipais) ? "Municipal" : null;


            FilterDefinition<Aeroporto> filtro_builder;

            if (String.IsNullOrEmpty(tipoAero))
            {
                filtro_builder = Builders<Aeroporto>.Filter.NearSphere(x => x.loc, localizacao, distancia);
            }
            else
            {
                filtro_builder = Builders<Aeroporto>.Filter.NearSphere(x => x.loc, localizacao, distancia) & Builders<Aeroporto>.Filter.Eq(x => x.type, tipoAero);
            };

            var listaAeroportos = await repository.GetManyAsync(filtro_builder);

            foreach (var doc in listaAeroportos)
            {
                var aero = new Coordenada(doc.name,
                    Convert.ToString(doc.loc.Coordinates.Latitude).Replace(",", "."),
                    Convert.ToString(doc.loc.Coordinates.Longitude).Replace(",", "."));
                aeroportosProximos.Add(aero);
            }

            return Json(aeroportosProximos);
        }

        private Coordenada ObterCoordenadasDaLocalizacao(string endereco)
        {
            string url = $"http://maps.google.com/maps/api/geocode/json?key={this.apiKey}&address={endereco}";

            var coord = new Coordenada("Não Localizado", "-10", "-10");
            var response = new WebClient().DownloadString(url);
            var googleGeocode = JsonConvert.DeserializeObject<GoogleGeocodeResponse>(response);

            if (googleGeocode.status == "OK")
            {
                coord.Nome = googleGeocode.results[0].formatted_address;
                coord.Latitude = googleGeocode.results[0].geometry.location.lat;
                coord.Longitude = googleGeocode.results[0].geometry.location.lng;
            }

            return coord;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}