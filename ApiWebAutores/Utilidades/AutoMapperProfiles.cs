using ApiWebAutores.DTOs;
using ApiWebAutores.Entidades;
using AutoMapper;
using System;
using System.Collections.Generic;

namespace ApiWebAutores.Utilidades
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles() {
            CreateMap<AutorDTO, Autor>();
            CreateMap<Autor, AutorListDTO>();
            CreateMap<LibroCreateDTO, Libro>();
            //.ForMember(libro => libro.autores, opc => opc.MapFrom(MapAutoresLibros));
            CreateMap<Libro, LibroDTO>();
                //.ForMember(libro => libro.autores, opc => opc.MapFrom(MapLibroDTOAutores));
            CreateMap<ComentarioCreateDTO, Comentario>();
            CreateMap<Comentario, ComentarioDTO>();
            /*
            CreateMap<LibroCreateDTO, Libro>()
                .ForMember(libro => libro.Autores, opciones => opciones.MapFrom(MapAutoresLibros));*/

        } 
        private List<AutorLibro> MapAutoresLibros(LibroCreateDTO libroCreateDTO, Libro libro)
        {
            var res = new List<AutorLibro>();
            if (libroCreateDTO.AutoresIds == null)
            {
                return res;
            }

            foreach(var autorId in libroCreateDTO.AutoresIds)
            {
                res.Add(new AutorLibro() { AutorId = autorId });
            }

            return res;
        } 

        private List<AutorListDTO> MapLibroDTOAutores(Libro libro, LibroDTO libroDTO)
        {
            var res = new List<AutorListDTO>();

            if (libro.Autores == null) { return res; }

            foreach(var autorLibro in libro.Autores)
            {
                res.Add(new AutorListDTO() { 
                    Id = autorLibro.Autor.Id,
                    Nombre = autorLibro.Autor.Nombre
                });
            }

            return res;
        }
    }
}
