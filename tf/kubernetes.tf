data "digitalocean_kubernetes_versions" "cluster_01_version" {
  version_prefix = var.cluster_auto_upgrade_verison
}

resource "digitalocean_kubernetes_cluster" "cluster_01" {
  name         = var.cluster_name
  region       = var.cluster_region
  auto_upgrade = true
  version      = var.k8s_version

  tags = local.all_cluster_tags

  maintenance_policy {
    start_time = "08:00" //3 AM EST
    day        = "monday"
  }

  node_pool {
    name       = var.node_pool_name
    size       = var.node_size
    auto_scale = true
    min_nodes  = var.node_pool_min_size
    max_nodes  = var.node_pool_max_size
    tags       = local.all_node_pool_tags
  }
}

resource "kubernetes_v1_secret" "ecommerce_app_secret" {
  depends_on = [digitalocean_database_db.database_01_mysql, digitalocean_database_user.database_01_mysql_user, digitalocean_database_db.database_01_mysql_name]

  metadata {
    name      = "ecommerce-database"
    namespace = "ecommerce"
  }

  type = "Opaque"

  data = {
    "connection-string" = base64encode("Server=${digitalocean_database_db.database_01_mysql.private_host};User ID=${var.database_user};Password=${digitalocean_database_user.database_01_mysql_user.password};Database=${var.database_name}")
  }
}