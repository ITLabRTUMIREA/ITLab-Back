docker restart scbs-postgres
docker run --name scbs-postgres -e POSTGRES_PASSWORD=password -p 5432:5432 -d postgres
