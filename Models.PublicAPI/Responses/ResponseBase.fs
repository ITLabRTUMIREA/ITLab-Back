namespace Models.PublicAPI

module Response =
    type StatusCode = Unknown = 0 | OK = 1

    type Base = {Status: StatusCode}

