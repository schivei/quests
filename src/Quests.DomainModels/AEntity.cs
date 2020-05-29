using Dgraph4Net;
using Dgraph4Net.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;

#nullable enable

namespace Quests.DomainModels
{
    [DataContract]
    public abstract class AEntity : IEntity
    {
        protected AEntity()
        {
            _dgraphType = new[] { DgraphExtensions.GetDType(this) };
            Id = Uid.NewUid();
            User = string.Empty;
        }

        [JsonProperty("uid"), Required, DataMember(Name = "uid")]
        public virtual Uid Id { get; set; }

        private ICollection<string> _dgraphType;

        [JsonProperty("dgraph.type"), MinLength(1), Required(AllowEmptyStrings = false), DataMember(Name = "dgraph.type")]
        public ICollection<string> DgraphType
        {
            get
            {
                var dtype = DgraphExtensions.GetDType(this);
                if (_dgraphType.All(dt => dt != dtype))
                    _dgraphType.Add(dtype);

                return _dgraphType;
            }
            set
            {
                var dtype = DgraphExtensions.GetDType(this);
                if (value.All(dt => dt != dtype))
                    value.Add(dtype);

                _dgraphType = value;
            }
        }

        [JsonProperty("creation_date"), DataMember(Name = "creation_date")]
        [DateTimePredicate(Token = DateTimeToken.Hour, Upsert = true), Required]
        public DateTimeOffset CreationDate { get; set; } = DateTimeOffset.Now;

        [JsonProperty("user"), DataMember(Name = "user")]
        [StringPredicate(Token = StringToken.Exact), Required(AllowEmptyStrings = false)]
        [MinLength(3)]
        public string User { get; set; }
    }
}

#nullable restore
