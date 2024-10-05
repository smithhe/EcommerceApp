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