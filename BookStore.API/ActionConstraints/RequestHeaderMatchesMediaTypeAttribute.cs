using System;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace BookStore.API.ActionConstraints
{
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class RequestHeaderMatchesMediaTypeAttribute : Attribute, IActionConstraint
    {
        private readonly MediaTypeCollection _mediaTypes = new MediaTypeCollection();
        private readonly string _requestedHeaderToMatch;

        public RequestHeaderMatchesMediaTypeAttribute(string requestedHeaderToMatch,
            string mediaType, params string[] otherMediaTypes)
        {
            _requestedHeaderToMatch = requestedHeaderToMatch ??
                throw new ArgumentNullException(nameof(requestedHeaderToMatch));

            if (MediaTypeHeaderValue.TryParse(mediaType,
                out MediaTypeHeaderValue parsedMediaType))
            {
                _mediaTypes.Add(parsedMediaType);
            }
            else
            {
                throw new ArgumentException(nameof(mediaType));
            }

            foreach(var otherMediaType in otherMediaTypes)
            {
                if(MediaTypeHeaderValue.TryParse(otherMediaType,
                    out MediaTypeHeaderValue parsedOtherMediaType))
                {
                    _mediaTypes.Add(parsedOtherMediaType);
                }
                else
                {
                    throw new ArgumentException(nameof(otherMediaTypes));
                }
            }
        }

        public int Order => 0;

        public bool Accept(ActionConstraintContext context)
        {
            var requestHeader = context.RouteContext.HttpContext.Request.Headers;
            if (!requestHeader.ContainsKey(_requestedHeaderToMatch))
            {
                return false;
            }

            var parsedRequestMediaType = new MediaType(requestHeader[_requestedHeaderToMatch]);

            foreach(var mediaType in _mediaTypes)
            {
                var parsedMediaType = new MediaType(mediaType);
                if (parsedRequestMediaType.Equals(parsedMediaType))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
