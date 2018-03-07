using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Xpinn.SportsGo.Util.Portable.Enums;

namespace Xpinn.SportsGo.MVC.Models
{

    public static class Helper
    {
        public static byte[] getBytesFromFile(HttpPostedFileBase file)
        {
            byte[] fileData = null;
            using (var binaryReader = new BinaryReader(file.InputStream))
            {
                fileData = binaryReader.ReadBytes(file.ContentLength);
            }
            return fileData;
        }

        public static int getFileType(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            switch (extension)
            {
                case ".jpg":
                    return (int)TipoArchivo.Imagen;
                case ".png":
                    return (int)TipoArchivo.Imagen;
                case ".mp4":
                    return (int)TipoArchivo.Video;
                case ".mov":
                    return (int)TipoArchivo.Video;
                case ".avi":
                    return (int)TipoArchivo.Video;
                case ".3gp":
                    return (int)TipoArchivo.Video;
                default:
                    return (int)TipoArchivo.SinTipoArchivo;
            }
        }

        public static Result<Object> returnErrorSesion()
        {
            Result<Object> result = new Result<Object>();

            result.Success = false;
            result.Message = "Error al obtener el usuario logueado";
            result.Path = "Authenticate/";
            return result;
        }

        public static Result<Object> returnErrorWrongUser()
        {
            Result<Object> result = new Result<Object>();

            result.Success = false;
            result.Message = "NOTI_USER_PASS_INCORRECT";
            return result;
        }


        public static Result<Object> returnErrorList(int langKey)
        {
            Result<Object> result = new Result<Object>();
            result.Success = false;

            switch (langKey)
            {
                case 1:
                    result.Message = "Error al cargar la lista";
                    break;
                case 2:
                    result.Message = "Error loading list";
                    break;
                case 3:
                    result.Message = "Lista de carga de erro";
                    break;
                default:
                    result.Message = "Error al cargar la lista";
                    break;
            }
            
            return result;
        }

        public static Result<Object> returnNoDataList(int langKey)
        {
            Result<Object> result = new Result<Object>();
            result.Success = false;
            switch (langKey)
            {
                case 1:
                    result.Message = "No se encontraron datos";
                    break;
                case 2:
                    result.Message = "No data found";
                    break;
                case 3:
                    result.Message = "Não foram encontrados dados";
                    break;
                default:
                    result.Message = "No se encontraron datos";
                    break;
            }
            return result;
        }

        public static Result<Object> returnErrorObj(int langKey)
        {
            Result<Object> result = new Result<Object>();
            result.Success = false;
            switch (langKey)
            {
                case 1:
                    result.Message = "Error al obtener la información";
                    break;
                case 2:
                    result.Message = "Failed to get information";
                    break;
                case 3:
                    result.Message = "Erro ao obter informações";
                    break;
                default:
                    result.Message = "Error al obtener la información";
                    break;
            }
            return result;
        }

        public static Result<Object> returnErrorSaveObj(int langKey)
        {
            Result<Object> result = new Result<Object>();
            result.Success = false;
            switch (langKey)
            {
                case 1:
                    result.Message = "No se logró guardar la información";
                    break;
                case 2:
                    result.Message = "Failed to save information";
                    break;
                case 3:
                    result.Message = "Não foi possível salvar as informações";
                    break;
                default:
                    result.Message = "No se logró guardar la información";
                    break;
            }
            return result;
        }

        public static Result<Object> returnErrorFile(int langKey)
        {
            Result<Object> result = new Result<Object>();
            result.Success = false;
            switch (langKey)
            {
                case 1:
                    result.Message = "Error al cargar el archivo";
                    break;
                case 2:
                    result.Message = "Error loading file";
                    break;
                case 3:
                    result.Message = "Erro ao carregar arquivo";
                    break;
                default:
                    result.Message = "Error al cargar el archivo";
                    break;
            }
            return result;
        }

        public static Result<Object> returnErrorDelete(int langKey)
        {
            Result<Object> result = new Result<Object>();
            result.Success = false;
            switch (langKey)
            {
                case 1:
                    result.Message = "No se logró eliminar la información";
                    break;
                case 2:
                    result.Message = "Failed to delete information";
                    break;
                case 3:
                    result.Message = "Não foi possível apagar informações";
                    break;
                default:
                    result.Message = "No se logró eliminar la información";
                    break;
            }
            return result;
        }

        public static Result<Object> returnSuccessObj(int langKey)
        {
            Result<Object> result = new Result<Object>();
            result.Success = true;
            switch (langKey)
            {
                case 1:
                    result.Message = "El registro se ha guardado con éxito";
                    break;
                case 2:
                    result.Message = "Registration saved successfully";
                    break;
                case 3:
                    result.Message = "O registro foi salva com sucesso";
                    break;
                default:
                    result.Message = "El registro se ha guardado con éxito";
                    break;
            }
            return result;
        }

        public static Result<Object> returnSuccessDeleteObj(int langKey)
        {
            Result<Object> result = new Result<Object>();
            result.Success = true;
            switch (langKey)
            {
                case 1:
                    result.Message = "El registro se ha eliminado con éxito";
                    break;
                case 2:
                    result.Message = "Registration successfully deleted";
                    break;
                case 3:
                    result.Message = "O registro foi removido com sucesso";
                    break;
                default:
                    result.Message = "El registro se ha eliminado con éxito";
                    break;
            }
            return result;
        }

        public static Result<Object> returnDeleteSuccess(int langKey)
        {
            Result<Object> result = new Result<Object>();
            result.Success = true;
            switch (langKey)
            {
                case 1:
                    result.Message = "Se ha eliminado con éxito";
                    break;
                case 2:
                    result.Message = "Successfully deleted";
                    break;
                case 3:
                    result.Message = "Ele foi removido com sucesso";
                    break;
                default:
                    result.Message = "Se ha eliminado con éxito";
                    break;
            }
            return result;
        }

        public static Result<Object> returnSendMailSuccess()
        {
            Result<Object> result = new Result<Object>();
            result.Success = true;
            return result;
        }

        public static Result<Object> returnSendMailError()
        {
            Result<Object> result = new Result<Object>();
            result.Success = false;
            return result;
        }

        public static Result<Object> returnErrorDeletePlan(int langKey)
        {
            Result<Object> result = new Result<Object>();
            result.Success = false;
            switch (langKey)
            {
                case 1:
                    result.Message = "No puedes eliminar este plan, es el único default o ya está siendo utilizado por algún usuario";
                    break;
                case 2:
                    result.Message = "You can not delete this plan, it is the only default or it is already being used by some user";
                    break;
                case 3:
                    result.Message = "Você não pode excluir este plano é o único padrão ou por um usuário e está sendo usado";
                    break;
                default:
                    result.Message = "No puedes eliminar este plan, es el único default o ya está siendo utilizado por algún usuario";
                    break;
            }
            return result;
        }

        public static Result<Object> returnErrorUpdatePlan(int langKey)
        {
            Result<Object> result = new Result<Object>();
            result.Success = false;
            switch (langKey)
            {
                case 1:
                    result.Message = "No puedes quedarte sin planes default para este perfil";
                    break;
                case 2:
                    result.Message = "You can not run out of default plans for this profile";
                    break;
                case 3:
                    result.Message = "Você não pode ficar sem planos padrão para este perfil";
                    break;
                default:
                    result.Message = "No puedes quedarte sin planes default para este perfil";
                    break;
            }
            return result;
        }

        public static Result<Object> returnSuccessfulPayment(int langKey)
        {
            Result<Object> result = new Result<Object>();
            result.Success = true;
            switch (langKey)
            {
                case 1:
                    result.Message = "Gracias por adquirir nuestros servicios, le notificaremos cuando se haya confirmado el pago!";
                    break;
                case 2:
                    result.Message = "Thank you for purchasing our services, we will notify you when payment has been confirmed!";
                    break;
                case 3:
                    result.Message = "Obrigado por adquirir nossos serviços, iremos notificá-lo quando o pagamento é confirmado!";
                    break;
                default:
                    result.Message = "Gracias por adquirir nuestros servicios, le notificaremos cuando se haya confirmado el pago!";
                    break;
            }
            result.Message = "Gracias por adquirir nuestros servicios, le notificaremos cuanto se haya confirmado el pago!";
            return result;
        }

        public static Result<Object> returnErrorUnauthorizedByPlan(int langKey)
        {
            Result<Object> result = new Result<Object>();
            result.Success = false;
            result.AuthorizedByPlan = false;
            switch (langKey)
            {
                case 1:
                    result.Message = "Esta opcion no está disponible en tu plan!";
                    break;
                case 2:
                    result.Message = "This option is not available in your plan!";
                    break;
                case 3:
                    result.Message = "Esta opção não está disponível no seu plano!";
                    break;
                default:
                    result.Message = "Esta opcion no está disponible en tu plan!";
                    break;
            }
            return result;
        }
    }


    public class Result <T>
    {
        public Boolean Success { get; set; }
        public String Message { get; set; }
        public String Path { get; set; }
        public T obj { get; set; }
        public List<T> list { get; set; }
        public Boolean AuthorizedByPlan { get; set; }
        public Result()
        {
            this.Success = true;
            this.AuthorizedByPlan = true;
            this.list = new List<T>();
            this.Message = "";
        }

        public Result(T o) : this()
        {
            this.obj = o;
        }
        public Result(List<T> l) : this()
        {
            this.list = l;
        }
    }
    

    public class Filtro
    {
        public Int32? StartIndex { get; set; }
        public Int32? EndIndex { get; set; }
        public String Name { get; set; }
        public Int32? DatoInt { get; set; }
    }

}