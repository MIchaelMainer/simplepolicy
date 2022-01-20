package simplepolicy.GET.me
default allowed = false # Supporting the secure by default principle of authorization

# only authorize one user
allowed {
  input.user.id == "011a88bc-7df9-4d92-ba1f-2ff319e101e1"
}

# authorize users in the sales department
# allowed {
#   input.user.attributes.properties.department == "Sales"
# }
