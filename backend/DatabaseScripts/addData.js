use("DeviceManagementDB")


if (db.Users.countDocuments() === 0) {
    db.Users.insertMany([
        { name: "Iacob Alex", role: "Fullstack Developer", location: "Cluj-Napoca", email: "alexandruiacob01@gmail.com", passwordHash: "" },
        { name: "Andrei Ionut", role: "Manager", location: "Timisoara", email: "andreiionut@gmail.com", passwordHash: "" },
        { name: "Oana Nagy", role: "QA Developer", location: "Bucuresti", email: "oananagy@gmail.com", passwordHash: "" }
    ]);
}


if (db.Devices.countDocuments() === 0) {
    db.Devices.insertMany([
        { name: "iPhone 15 Pro", manufacturer: "Apple", type: "phone", os: "iOS", osVersion: "17.0", processor: "A17 Pro", ram: 8, description: "A premium Apple smartphone with a titanium design and advanced camera system.", assignedUserId: null },
        { name: "Galaxy S24", manufacturer: "Samsung", type: "phone", os: "Android", osVersion: "14.0", processor: "Snapdragon 8 Gen 3", ram: 12, description: "A flagship Samsung smartphone with AI-powered features and a bright AMOLED display.", assignedUserId: null },
        { name: "iPad Pro 12.9", manufacturer: "Apple", type: "tablet", os: "iPadOS", osVersion: "17.0", processor: "M2", ram: 16, description: "A powerful Apple tablet designed for professional use with a large Liquid Retina display.", assignedUserId: null },
        { name: "Pixel 8", manufacturer: "Google", type: "phone", os: "Android", osVersion: "14.0", processor: "Tensor G3", ram: 8, description: "A Google smartphone focused on computational photography and clean Android experience.", assignedUserId: null },
        { name: "Surface Pro 9", manufacturer: "Microsoft", type: "tablet", os: "Windows", osVersion: "11", processor: "Intel Core i5", ram: 16, description: "A versatile Microsoft tablet that doubles as a laptop, ideal for business productivity.", assignedUserId: null }
    ]);
}
