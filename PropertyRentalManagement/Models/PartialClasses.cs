using Microsoft.AspNetCore.Mvc;
using PropertyRentalManagement.Models.Metadata;

namespace PropertyRentalManagement.Models
{
    [ModelMetadataType(typeof(StatusMetadata))]
    public partial class Status { }

    [ModelMetadataType(typeof(UserRoleMetadata))]
    public partial class UserRole { }

    [ModelMetadataType(typeof(VMLoginMetadata))]
    public partial class VMLogin { }

    [ModelMetadataType(typeof(BuildingMetadata))]
    public partial class Building { }

    [ModelMetadataType(typeof(ApartmentMetadata))]
    public partial class Apartment { }

    [ModelMetadataType(typeof(ApartmentTypeMetadata))]
    public partial class ApartmentType { }

    [ModelMetadataType(typeof(UserMetadata))]
    public partial class User { }

    [ModelMetadataType(typeof(AppointmentMetadata))]
    public partial class Appointment { }

    [ModelMetadataType(typeof(EventMetadata))]
    public partial class Event { }

    [ModelMetadataType(typeof(MessageMetadata))]
    public partial class Message { }
}
