resource "digitalocean_firewall" "cluster_01_firewall" {
  name = var.firewall_name

  tags = local.all_firewall_tags

  depends_on = [digitalocean_kubernetes_cluster.cluster_01]

  inbound_rule {
    protocol         = "tcp"
    port_range       = "80"
    source_addresses = ["0.0.0.0/0", "::/0"]
  }

  inbound_rule {
    protocol         = "tcp"
    port_range       = "443"
    source_addresses = ["0.0.0.0/0", "::/0"]
  }

  inbound_rule {
    protocol         = "icmp"
    source_addresses = ["0.0.0.0/0", "::/0"]
  }

  outbound_rule {
    protocol              = "tcp"
    port_range            = "53"
    destination_addresses = ["0.0.0.0/0", "::/0"]
  }

  outbound_rule {
    protocol              = "udp"
    port_range            = "53"
    destination_addresses = ["0.0.0.0/0", "::/0"]
  }

  outbound_rule {
    protocol              = "icmp"
    destination_addresses = ["0.0.0.0/0", "::/0"]
  }
}

resource "digitalocean_database_firewall" "database_01_mysql_firewall" {
  cluster_id = digitalocean_database_cluster.database_01_mysql.id

  depends_on = [digitalocean_database_cluster.database_01_mysql, digitalocean_kubernetes_cluster.cluster_01]

  rule {
    type  = "k8s"
    value = digitalocean_kubernetes_cluster.cluster_01.id
  }
}
