try {
    use("DeviceManagementDB")



    if (!db.getCollectionNames().includes("Users")) {
        db.createCollection("Users");
    }

    if (!db.getCollectionNames().includes("Devices")) {
        db.createCollection("Devices");
    }
} catch (err) {
    print("Error creating the collections",err)
}