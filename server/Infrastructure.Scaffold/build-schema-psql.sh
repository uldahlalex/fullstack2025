#yay -S postgresql
psql -h localhost -p 5432 -d testdb -U testuser -f schema.sql