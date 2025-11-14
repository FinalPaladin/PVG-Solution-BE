using Microsoft.AspNetCore.Mvc;
using PVG.Core.BaseModels;

namespace PVG.Web
{
    [Route("pvg")]
    [ApiController]
    public class PVGControllerBase : ControllerBase
    {
        public PVGControllerBase()
        {
        }

        protected ObjectResult SuccessResponse(BaseResponse response)
        {
            return StatusCode(StatusCodes.Status200OK, response);
        }

        protected ObjectResult BadRequestResponse(BaseResponse response)
        {
            return StatusCode(StatusCodes.Status400BadRequest, response);
        }

        protected ObjectResult NotFoundResponse(BaseResponse response)
        {
            return StatusCode(StatusCodes.Status404NotFound, response);
        }

        protected ObjectResult InternalServerError(BaseResponse response)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        protected ObjectResult ReturnData(BaseResponse response)
        {
            switch (response.StatusCode)
            {
                case 200:
                    {
                        return SuccessResponse(response);
                    }
                    ;
                case 400:
                    {
                        return BadRequestResponse(response);
                    }
                    ;
                case 404:
                    {
                        return NotFoundResponse(response);
                    }
                    ;
                case 500:
                    {
                        return InternalServerError(response);
                    }
                    ;
                default: return null;
            }
        }


        protected ObjectResult SuccessResponse<T>(BaseResponse<T> response)
        {
            return StatusCode(StatusCodes.Status200OK, response);
        }

        protected ObjectResult BadRequestResponse<T>(BaseResponse<T> response)
        {
            return StatusCode(StatusCodes.Status400BadRequest, response);
        }

        protected ObjectResult NotFoundResponse<T>(BaseResponse<T> response)
        {
            return StatusCode(StatusCodes.Status404NotFound, response);
        }

        protected ObjectResult InternalServerError<T>(BaseResponse<T> response)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        protected ObjectResult ReturnData<T>(BaseResponse<T> response)
        {
            switch (response.StatusCode)
            {
                case 200:
                    {
                        return SuccessResponse(response);
                    }
                    ;
                case 400:
                    {
                        return BadRequestResponse(response);
                    }
                    ;
                case 404:
                    {
                        return NotFoundResponse(response);
                    }
                    ;
                case 500:
                    {
                        return InternalServerError(response);
                    }
                    ;
                default: return null;
            }
        }
    }
}
