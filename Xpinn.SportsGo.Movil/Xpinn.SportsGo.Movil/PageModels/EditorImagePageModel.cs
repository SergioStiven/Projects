using Acr.UserDialogs;
using FFImageLoading.Cache;
using FFImageLoading.Forms;
using FreshMvvm;
using PCLStorage;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xpinn.SportsGo.Entities;
using Xpinn.SportsGo.Movil.Abstract;
using Xpinn.SportsGo.Movil.Infraestructure;
using Xpinn.SportsGo.Movil.Models;
using Xpinn.SportsGo.Movil.Resources;
using Xpinn.SportsGo.Services;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.Movil.PageModels
{
    class EditorImagePageModel : BasePageModel
    {
        ArchivosServices _archivoServices;

        public ImagenEditorModel SourceModel { get; set; }

        public EditorImagePageModel()
        {
            _archivoServices = new ArchivosServices();
        }

        public override void Init(object initData)
        {
            base.Init(initData);
            SourceModel = initData as ImagenEditorModel;
        }

        public ICommand ImagenSavedCommand
        {
            get
            {
                return new FreshAwaitCommand(async (parameter, tcs) =>
                {
                    string pathNewImagen = parameter as string;

                    var config = new ProgressDialogConfig()
                                       .SetTitle(SportsGoResources.Cargando)
                                       .SetIsDeterministic(false);

                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        config.SetMaskType(MaskType.Black);
                    }
                    else
                    {
                        config.SetMaskType(MaskType.Gradient);
                    }

                    using (Dialogs.Progress(config))
                    {
                        if (Device.RuntimePlatform == Device.Android) // En android se debe eliminar la transparencia de las imagenes
                        {
                            IHelperImagen convertImagen = FreshIOC.Container.Resolve<IHelperImagen>();
                            await convertImagen.DeleteTransparencyFromAnImage(pathNewImagen, 100, FormatoImagen.Jpeg);
                        }
                        else if (Device.RuntimePlatform == Device.iOS) // En iOS la libreria la trollea y guarda la imagen en una ubicacion deprecated y no de facil acceso, asi que toco resolver con el stream y copiar el archivo
                        {
                            IFile createdFile = await FileSystem.Current.LocalStorage.CreateFileAsync("EditorImagenes", CreationCollisionOption.ReplaceExisting);

                            using (Stream streamToWrite = await createdFile.OpenAsync(FileAccess.ReadAndWrite))
                            using (Stream streamToNewFile = parameter as Stream)
                            {
                                await streamToNewFile.CopyToAsync(streamToWrite);
                            }

                            pathNewImagen = createdFile.Path;
                        }

                        try
                        {
                            if (SourceModel.Persona.Consecutivo > 0 && !SourceModel.EsPrimerRegistro && !SourceModel.EsEvento)
                            {
                                IFile file = await FileSystem.Current.GetFileFromPathAsync(pathNewImagen);
                                using (Stream streamSource = await file.OpenAsync(FileAccess.Read))
                                {
                                    WrapperSimpleTypesDTO wrapper = null;

                                    if (SourceModel.EsImagenPerfil)
                                    {
                                        wrapper = await AsignarImagenPerfil(streamSource);
                                    }
                                    else if (SourceModel.EsImagenBanner)
                                    {
                                        wrapper = await AsignarImagenBanner(streamSource);
                                    }

                                    if (wrapper != null && wrapper.Exitoso)
                                    {
                                        int codigoArchivo = Convert.ToInt32(wrapper.ConsecutivoArchivoCreado);
                                        ImagenEditorModel newImagen = new ImagenEditorModel
                                        {
                                            Persona = SourceModel.Persona,
                                            Source = pathNewImagen,
                                            CodigoArchivoCreado = codigoArchivo,
                                            EsPrimerRegistro = SourceModel.EsPrimerRegistro,
                                            EsImagenBanner = SourceModel.EsImagenBanner,
                                            EsImagenPerfil = SourceModel.EsImagenPerfil,
                                            CodigoEvento = SourceModel.CodigoEvento,
                                            EsEvento = SourceModel.EsEvento
                                        };

                                        if (SourceModel.EsImagenPerfil)
                                        {
                                            SourceModel.Persona.CodigoArchivoImagenPerfil = codigoArchivo;
                                        }
                                        else
                                        {
                                            SourceModel.Persona.CodigoArchivoImagenBanner = codigoArchivo;
                                        }

                                        await CoreMethods.PopPageModel(newImagen);
                                    }
                                    else if (SourceModel.EsImagenBanner)
                                    {
                                        await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorAsignarImagenes, "OK");
                                    }
                                }

                            }
                            else
                            {
                                ImagenEditorModel newImagen = new ImagenEditorModel
                                {
                                    Persona = SourceModel.Persona,
                                    Source = pathNewImagen,
                                    EsPrimerRegistro = SourceModel.EsPrimerRegistro,
                                    EsImagenBanner = SourceModel.EsImagenBanner,
                                    EsImagenPerfil = SourceModel.EsImagenPerfil,
                                    CodigoEvento = SourceModel.CodigoEvento,
                                    EsEvento = SourceModel.EsEvento
                                };

                                await CoreMethods.PopPageModel(newImagen);
                            }
                        }
                        catch (Exception)
                        {
                            await CoreMethods.DisplayAlert(SportsGoResources.Error, SportsGoResources.ErrorAsignarImagenes, "OK");
                        }
                    }

                    tcs.SetResult(true);
                });
            }
        }

        async Task<WrapperSimpleTypesDTO> AsignarImagenPerfil(Stream streamPerfil)
        {
            WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO();

            if (streamPerfil != null)
            {
                int codigoArchivo = SourceModel.Persona.CodigoArchivoImagenPerfil.HasValue ? SourceModel.Persona.CodigoArchivoImagenPerfil.Value : 0;

                if (IsNotConnected) return null;
                wrapper = await _archivoServices.AsignarImagenPerfilPersona(SourceModel.Persona.Consecutivo, codigoArchivo, streamPerfil);

                if (wrapper != null && wrapper.Exitoso)
                {
                    await CachedImage.InvalidateCache(SourceModel.Persona.UrlImagenPerfil, CacheType.All, true);
                    SourceModel.Persona.CodigoArchivoImagenPerfil = Convert.ToInt32(wrapper.ConsecutivoArchivoCreado);
                }
            }

            return wrapper;
        }

        async Task<WrapperSimpleTypesDTO> AsignarImagenBanner(Stream streamBanner)
        {
            WrapperSimpleTypesDTO wrapper = new WrapperSimpleTypesDTO();

            if (streamBanner != null)
            {
                int codigoArchivo = SourceModel.Persona.CodigoArchivoImagenBanner.HasValue ? SourceModel.Persona.CodigoArchivoImagenBanner.Value : 0;

                if (IsNotConnected) return null;
                wrapper = await _archivoServices.AsignarImagenBannerPersona(SourceModel.Persona.Consecutivo, codigoArchivo, streamBanner);

                if (wrapper != null && wrapper.Exitoso)
                {
                    await CachedImage.InvalidateCache(SourceModel.Persona.UrlImagenBanner, CacheType.All, true);
                    SourceModel.Persona.CodigoArchivoImagenBanner = Convert.ToInt32(wrapper.ConsecutivoArchivoCreado);
                }
            }

            return wrapper;
        }
    }
}
