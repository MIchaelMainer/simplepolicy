package simplepolicy.GET.me
default allowed = false

# only authorize one user
# allowed {
#   input.user.id == "011a88bc-7df9-4d92-ba1f-2ff319e101e1"
# }

allowed {
  input.user.attributes.properties.department == "Sales"
}
