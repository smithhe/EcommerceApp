resource "digitalocean_database_cluster" "database_01_mysql" {
  name       = var.database_cluster_name
  engine     = "mysql"
  version    = var.database_mysql_version
  size       = var.database_instance_size
  region     = var.database_region
  node_count = var.database_node_count
}

resource "digitalocean_database_db" "database_01_mysql_name" {
  cluster_id = digitalocean_database_cluster.database_01_mysql.id
  name       = var.database_name
}

resource "digitalocean_database_user" "database_01_mysql_user" {
  cluster_id = digitalocean_database_cluster.database_01_mysql.id
  name       = var.database_user
}
